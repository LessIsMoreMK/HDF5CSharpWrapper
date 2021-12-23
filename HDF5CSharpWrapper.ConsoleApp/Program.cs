using System;
using System.Collections.Generic;

namespace HDF5CSharpWrapper.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fileBasic = new File();
            var groups = new Groups();
            var datasets = new Datasets();
            var attributes = new Attributes();

            var file = fileBasic.CreateFile("t.h5");
            var file2 = fileBasic.OpenFile("test.h5");

            #region Groups

            var group = groups.CreateGroup(file, "Arrays");
            /*var group2 = groups.CreateGroup(file, "GroupName");
            var group3 = groups.CreateGroup(file, "second");
            var group4 = groups.CreateGroup(group2, "third2");*/

            #endregion

            #region Datasets

            /*int[] intArray = { 1, 2, 3, 4, 5 };
            int[,] intTwoDimArray = { { 1, 2 },
                                      { 3, 4 } };
            float[] doubleArray = { 25.0F };
            float[,] doubleTwoDimArray = { { 1.1F, 2.2F },
                                            { 3.3F, 4.4F} };
            byte[] byteArray = { 1, 0, 1 };
            byte[,] byteTwoDimArray = { { 1, 0 },
                                        { 1, 1 } };

            byte[,] charArray = { { 32, 33 },
                                     { 34, 35 } };

            


            var dataset = datasets.SetDataset<int>(group, "int", intArray);
            var dataset2 = datasets.SetDataset<int>(file, "intTwoDim", intTwoDimArray);
            var dataset3 = datasets.SetDataset<float>(file, "double", doubleArray);
            var dataset4 = datasets.SetDataset<float>(file, "doubleTwoDim", doubleTwoDimArray);
            var dataset5 = datasets.SetDataset<byte>(file, "byte", byteArray);
            var dataset6 = datasets.SetDataset<byte>(file, "byteTwoDim", byteTwoDimArray);
            var dataset7 = datasets.SetDataset<byte>(file, "charTwoDim", charArray);
            var dataset8 = datasets.SetDataset<sbyte>(file, "imageColor", imageColorArray);

            var dset = datasets.GetDataset<float>(file2, "float");
            var dset2 = datasets.GetDataset<float>(file2, "/Arrays/float");
            var dset3 = datasets.GetDataset<Int32>(file2, "/int");
            var dset4 = datasets.GetDataset<Int32>(file2, "/Arrays/int");
            var dset5 = datasets.GetDataset<SByte>(file2, "/color_image");
            var dset6 = datasets.GetDataset<SByte>(file2, "/mono_image");
            var dset7 = datasets.GetDataset<Byte>(file2, "/bool");
            var dset8 = datasets.GetDataset<Byte>(file2, "/char");
            var dset9 = datasets.GetDataset<Byte>(file2, "/Arrays/char");


            var dset11 = datasets.GetDataset<string>(file2, "Arrays/vlen_string");
            var dset13 = datasets.GetDataset<string>(file2, "/vlen_string");*/


            sbyte[,,] imageColorArray = { { { 1, 2, 3}, {4, 5, 6} },
                                          { { 7, 8, 9}, {10, 11, 12} } };
            int[] intValues = new[] { 1, 2 };
            string[] stringArray = { "tu jest napis śćąęłóżźń", "asdf", "asdf" };
            string[] arr = { "IMAGE" };
            int[] arr2 = { 0, 255 };
            string[] arr3 = { "IMAGE_TRUECOLOR" };
            string[] arr4 = { "1.2" };
            string[] arr5 = { "INTERPLACE_PIXEL" };

            var dataset5 = datasets.SetDataset<sbyte>(file, "imageColor", imageColorArray);
            var attribut6 = attributes.SetAttribute<string>(file, "CLASS", arr, "imageColor");
            var attribut7 = attributes.SetAttribute<int>(file, "IMAGE_MINMAXRANGE", arr2, "imageColor");
            var attribut8 = attributes.SetAttribute<string>(file, "IMAGE_SUBCLASS", arr3, "imageColor");
            var attribut9 = attributes.SetAttribute<string>(file, "IMAGE_VERSION", arr4, "imageColor"); 
            var attribut10 = attributes.SetAttribute<string>(file, "INTERPLACE_MODE", arr5, "imageColor");

            var attribut = attributes.SetAttribute<int>(group, "attrname", intValues, "int");
            var attribut2 = attributes.SetAttribute<string>(group, "attrnamee", stringArray, "int");
            var attribute3 = attributes.GetAttribute<int>(group, "attrname",  "int");
            var attribute4 = attributes.GetAttribute<string>(group, "attrnamee",  "int");


            #endregion

            fileBasic.CloseFile(file);
            fileBasic.CloseFile(file2);
        }
    }
}
