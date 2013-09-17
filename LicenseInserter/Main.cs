/*
 * 
 *	Copyright (c) 2013, RichyHBM
 * 
 *	All rights reserved.
 *	
 *	Redistribution and use in source and binary forms, with or without
 *	modification, are permitted provided that the following conditions are met:
 *	    * Redistributions of source code must retain the above copyright
 *	      notice, this list of conditions and the following disclaimer.
 *	    * Redistributions in binary form must reproduce the above copyright
 *	      notice, this list of conditions and the following disclaimer in the
 *	      documentation and/or other materials provided with the distribution.
 *	    * Neither the name of the <organization> nor the
 *	      names of its contributors may be used to endorse or promote products
 *	      derived from this software without specific prior written permission.
 *	
 *	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 *	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 *	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 *	DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
 *	DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *	(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 *	LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 *	ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 * 
 */
using System;
using System.IO;

namespace LicenseInserter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Make sure the user supplies the location to the files
            if (args.Length < 2)
            {
                Console.WriteLine("Please supply source files location and file extension as an argument");
                return;
            }

            string fileExtension = args[0];
            string sourceFiles = args[1];
            string license = "";
            string fullPathname = AppDomain.CurrentDomain.BaseDirectory + "license.txt";

            try
            {
                //Load the license file as a string
                using (StreamReader sr = new StreamReader(fullPathname))
                {
                    license = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong reading the license file: " + e.Message);
                return;
            }

            //Get a list of all files in the supplied path
            string[] filePaths;

            try
            {
                filePaths = Directory.GetFiles(sourceFiles);
            }
            catch
            {
                Console.WriteLine("Given source directory was not found, please make sure you typed the right path");
                return;
            }

            foreach (string file in filePaths)
            {
                string fileContents = "";

                //Skip exe, dll, ... and only process the requested extension
                if (    file.ToLower().EndsWith(".exe") ||
                        file.ToLower().EndsWith(".dll") ||
                        file.ToLower().EndsWith(".so")  ||
                        file.ToLower().EndsWith(".a")   ||
                        !file.ToLower().EndsWith(fileExtension.ToLower()))
                {
                    continue;
                }

                try
                {
                    //Load each source file in the path as a string
                    using (StreamReader sr = new StreamReader(file))
                    {
                        fileContents = sr.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong reading: " + file + ": " + e.Message);
                }

                //Skip the file if it already has the license
                if (fileContents.Contains(license))
                {
                    Console.WriteLine("Skipping file: " + file);
                    continue;
                }

                //If not then add the license to the file contents
                fileContents = license + "\n" + fileContents;

                //Write the new file contents to the file
                try
                {
                    using (StreamWriter outfile = new StreamWriter(file, false))
                    {
                        outfile.Write(fileContents);
                    }
                    Console.WriteLine("License added to file: " + file);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong writing to " + file + ": " + e.Message);
                }
            }
        }
    }
}
