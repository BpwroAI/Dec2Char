using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string content = null;

        if (args.Length > 0)
        {
            // 実行ファイルのフルパスを取得
            string executablePath = Assembly.GetExecutingAssembly().Location;

            // ディレクトリのパスを取得
            string directoryPath = Path.GetDirectoryName(executablePath);
            string folderName = "Output";
            string combinedPath = Path.Combine(directoryPath, folderName);
            try
            {
                // ディレクトリが存在しない場合に作成
                if (!Directory.Exists(combinedPath))
                {
                    Directory.CreateDirectory(combinedPath);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"フォルダ作成時にエラーが発生しました: {ex.Message}");
            }

            foreach (var filePath in args)
            {
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"ファイルが読み込まれました: {filePath}");
                    // 必要に応じてファイルの内容を処理する
                    content = File.ReadAllText(filePath);
                    string fileName = Path.GetFileName(filePath);
                    string inputFilePath = filePath;
                    string outputFilePath = Path.Combine(combinedPath, fileName);

                    try
                    {
                        // ファイルの内容を読み込み
                        content = File.ReadAllText(filePath);

                        // 「bytes」または「Bytes」を検索
                        //Match match = Regex.Match(content, @"(?:bytes|Bytes)\s*\[(.*?)\]", RegexOptions.IgnoreCase);
                        var match = Regex.Match(content, @"(?i)bytes?""\s*:\s*\[(.*?)\]", RegexOptions.Singleline);
                        if (match.Success)
                        {
                            // 「[」と「]」の間の文字列を取得
                            string dataInsideBrackets = match.Groups[1].Value;

                            // 半角スペース、タブ、改行を削除
                            string cleanedData = Regex.Replace(dataInsideBrackets, @"\s", "");

                            // [,]を半角スペースに変換
                            cleanedData = cleanedData.Replace(",", " ");

                            // 数字をバイナリデータに変換
                            string[] numbers = cleanedData.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            byte[] binaryData = new byte[numbers.Length];

                            for (int i = 0; i < numbers.Length; i++)
                            {
                                if (int.TryParse(numbers[i], out int asciiCode))
                                {
                                    binaryData[i] = (byte)asciiCode;
                                }
                            }

                            // バイナリデータをファイルに保存
                            using (FileStream fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(binaryData, 0, binaryData.Length);
                            }

                        }
                        else
                        {
                            Console.WriteLine("指定の形式がテキスト内に見つかりませんでした。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"エラーが発生しました: {ex.Message}");
                    }
                }
            }
        }
        else
        {
             Console.WriteLine("ドラッグ＆ドロップされたファイルがありません。");
        }
        Console.WriteLine("処理が完了しました。エンターキーを押してください。");
        Console.ReadLine();
    }
}
