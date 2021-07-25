using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using HDF.PInvoke;
using HDF5CSharp;

namespace MeaTaste.Domain.Hdf5.Service
{
    public class FileReader
    {
        long fileId;
        IList<string> _labels;
        
        public long Hdf5OpenFile(string FileName)
        {
            fileId = 0;
            var status = H5.open();
            Console.WriteLine("HDF5 open =", status);

            fileId = H5F.open(FileName, H5F.ACC_RDONLY);
            Console.WriteLine("fileId =", fileId, "\n");
            return fileId;
        }
    }
}
