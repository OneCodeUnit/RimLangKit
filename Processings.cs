namespace RTK
{
    public partial class MainForm
    {
        private static void ChangeSize(TextBox sender)
        {
            Size size = TextRenderer.MeasureText(sender.Text, sender.Font);
            sender.Width = size.Width;
            sender.Height = size.Height;
        }

        private static void ChangeSize(Label sender)
        {
            Size size = TextRenderer.MeasureText(sender.Text, sender.Font);
            sender.Width = size.Width;
            sender.Height = size.Height;
        }

        private (int, int) ResultProcessing((bool, string) result)
        {
            int ProcessedCount = 0;
            int SkipCount = 0;
            if (result.Item1)
            {
                ProcessedCount++;
            }
            else
            {
                SkipCount++;
                InfoTextBox.AppendText(Environment.NewLine);
                InfoTextBox.AppendText(result.Item2);
            }
            progressBar.Value++;
            return (ProcessedCount, SkipCount);
        }
        private void XMLProcessing(string sender)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.xml", SearchOption.AllDirectories);
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Maximum = AllFiles.Length;

            foreach (string TempFile in AllFiles)
            {
                switch (sender)
                {
                    case "UniqueNames":
                        tempCount = ResultProcessing(Program.NamesProcessing(TempFile));
                        break;
                    case "Comments":
                        tempCount = ResultProcessing(Program.CommentsProcessing(TempFile));
                        break;
                    default:
                        tempCount = ResultProcessing(Program.CommentsProcessing(TempFile));
                        break;
                }
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }

        private void TXTProcessing(bool mode)
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            //Получение списка всех файлов в заданой директории и во всех вложенных подпапках за счёт SearchOption
            string[] AllFiles = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Maximum = AllFiles.Length;

            foreach (string TempFile in AllFiles)
            {
                tempCount = ResultProcessing(Program.TranscriptionProcessing(TempFile, mode));
                Count.Item1 += tempCount.Item1;
                Count.Item2 += tempCount.Item2;
            }
            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }

        private void LanguageProcessing()
        {
            InfoTextBox.Text = string.Empty;
            InfoTextBox.AppendText("Старт");
            (int, int) Count = (0, 0);
            (int, int) tempCount;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            progressBar.Maximum = 3;

            tempCount = ResultProcessing(Program.CheckConfig(DirectoryPath));
            Count.Item1 += tempCount.Item1;
            Count.Item2 += tempCount.Item2;
            if (tempCount.Item2 == 1)
            {
                return;
            }
            tempCount = ResultProcessing(Program.LoadConfig(DirectoryPath));
            Count.Item1 += tempCount.Item1;
            Count.Item2 += tempCount.Item2;
            if (tempCount.Item2 == 1)
            {
                return;
            }

            tempCount = ResultProcessing(Program.LoadFile());
            Count.Item1 += tempCount.Item1;
            Count.Item2 += tempCount.Item2;

            InfoTextBox.AppendText(Environment.NewLine + "Обработано: " + Count.Item1 + ". Пропущено: " + Count.Item2);
        }
    }
}