using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.dbWave
{
    public class DataFileWriter
    {
        public bool SaveDataFile(MeaExperiment experiment, ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {

            CreateDirectoryFromExperimentFileName(experiment);

            bool flag = false;
            string authorName = "Mahesh Chand";
            int age = 30;
            string bookTitle = "ADO.NET Programming using C#";
            bool mvp = true;
            double price = 54.99;
            string fileName = @"C:\temp\MC.bin"; 
            
            try
            {
                using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    // Write string   
                    binWriter.Write(authorName);
                    // Write string   
                    // Write integer  
                    binWriter.Write(age);
                    binWriter.Write(bookTitle);
                    // Write boolean  
                    binWriter.Write(mvp);
                    // Write double   
                    binWriter.Write(price);
                }

                flag = true;
            }
            catch (IOException ioexp)
            {
                Trace.WriteLine($"Error: {ioexp.Message}");
                flag = false;
            }

            return flag;
        }

        private void CreateDirectoryFromExperimentFileName(MeaExperiment experiment)
        {
            string directoryName = experiment.FileName.Substring(0, experiment.FileName.Length-3);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}
