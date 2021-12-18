namespace HDF5CSharpWrapper.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fileBasic = new File();
            var groups = new Groups();
            var datasets = new Datasets();

            var file = fileBasic.Create("t.h5");
            var file2 = fileBasic.Open("test.h5");

            #region Groups

            var group = groups.OpenGroup(file2, "/Arrays");
            var group2 = groups.CreateGroup(file, "GroupName");
            var group3 = groups.CreateGroup(file, "second");

            #endregion

            #region Datasets

            /*int[] intArray = { 1, 2, 3, 4, 5 };
            int[,] intTwoDimArray = { { 1, 2 },
                                      { 3, 4 } };
            double[] doubleArray = { 25.0 };
            double[,] doubleTwoDimArray = { { 1.1, 2.2 },
                                            { 3.3, 4.4} };
            byte[] byteArray = { 1, 0, 1 };
            byte[,] byteTwoDimArray = { { 1, 0 },
                                        { 1, 1 } };

            byte[,] charArray = { { 32, 33 },
                                     { 34, 35 } };

            sbyte[,,] imageColorArray = { { { 1, 2, 3}, {4, 5, 6} },
                                          { { 7, 8, 9}, {10, 11, 12} } };


            var dataset = datasets.SetDataset<int>(file, "int", intArray);
            var dataset2 = datasets.SetDataset<int>(file, "intTwoDim", intTwoDimArray);
            var dataset3 = datasets.SetDataset<double>(file, "double", doubleArray);
            var dataset4 = datasets.SetDataset<double>(file, "doubleTwoDim", doubleTwoDimArray);
            var dataset5 = datasets.SetDataset<byte>(file, "byte", byteArray);
            var dataset6 = datasets.SetDataset<byte>(file, "byteTwoDim", byteTwoDimArray);
            var dataset7 = datasets.SetDataset<byte>(file, "charTwoDim", charArray);
            var dataset8 = datasets.SetDataset<sbyte>(file, "imageColor", imageColorArray);

            var dset = datasets.GetDataset<Double>(file2, "float");
            var dset2 = datasets.GetDataset<Double>(file2, "/Arrays/float");
            var dset3 = datasets.GetDataset<Int32>(file2, "/int");
            var dset4 = datasets.GetDataset<Int32>(file2, "/Arrays/int");
            var dset5 = datasets.GetDataset<SByte>(file2, "/color_image");
            var dset6 = datasets.GetDataset<SByte>(file2, "/mono_image");
            var dset7 = datasets.GetDataset<Byte>(file2, "/bool");
            var dset8 = datasets.GetDataset<Byte>(file2, "/char");
            var dset9 = datasets.GetDataset<Byte>(file2, "/Arrays/char");

            var dset12 = datasets.RemoveDataset(file2, "/int");*/

            #endregion

            fileBasic.Close(file);
            fileBasic.Close(file2);
        }
    }
}
