using Microsoft.ML.OnnxRuntimeGenAI;
using System.Diagnostics;
using System.Text;

namespace EnglishToKana;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("第一引数にモデルのフォルダパスが必須です");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }
        //System
        var systemPrompt = File.ReadLines("systemPrompt.txt").First();

        //ファイルから空行を境界として配列に読み込む。その後、改行を除去
        string newLine = System.Environment.NewLine;
        string[] userPrompts = File.ReadAllText(@"userPrompts.txt").Split(new string[] { newLine + newLine }, StringSplitOptions.RemoveEmptyEntries);
        //string[] userPrompts = File.ReadAllText(@"../../../PrivateJunkData/20241010_UserMessageList.txt").Split(new string[] { newLine + newLine }, StringSplitOptions.RemoveEmptyEntries);
        userPrompts = userPrompts.Select(prompt => prompt.Replace(newLine, "")).ToArray();

        // プロンプトのテンプレート
        var promptTemple = "<|system|>{0}<|end|><|user|>{1}<|end|><|assistant|>";
        Console.WriteLine("Model Directory: " + args[0]);

        var sw = Stopwatch.StartNew();
        Model model = new Model(args[0]);
        Tokenizer tokenizer = new Tokenizer(model); //Modelを指定してトークナイザを作成
        sw.Stop();
        Console.WriteLine($"Modelロード時間(ms)：{sw.ElapsedMilliseconds}");

        sw = Stopwatch.StartNew();

        //パラメータ設定
        GeneratorParams generatorParams = new GeneratorParams(model);   //Modelを指定して生成パラメータを作成
        generatorParams.SetSearchOption("max_length", 4096);
        //generatorParams.SetSearchOption("max_length", 4096*2);

        generatorParams.TryGraphCaptureWithMaxBatchSize(1);

        foreach (string userPrompt in userPrompts)
        {
            var userInstruct = "次の英単語を日本語のカタカナ読みへ変換せよ";
            var finalPromptText = string.Format(promptTemple, systemPrompt, $"{userInstruct}{Environment.NewLine}###{Environment.NewLine}{userPrompt}");

            sw.Start();
            //プロンプトのトークン化
            //Sequences sequences = tokenizer.Encode(prompt);
            generatorParams.SetInputSequences(tokenizer.Encode(finalPromptText));    //プロンプト変更

            using var generator = new Generator(model, generatorParams);    //Modelと生成パラメータを指定して生成器を作成
            using var tokenizerStream = tokenizer.CreateStream();

            Console.WriteLine($"Q.{Environment.NewLine}{userPrompt}");
            Console.WriteLine($"AI.");
            var response = Generate(generator, tokenizer);
            Console.WriteLine(response);
            Console.WriteLine();
            sw.Stop();
            Console.WriteLine($"所要時間: {sw.Elapsed.TotalSeconds:F2} 秒 文字数：{userPrompt.Length} -> {response.Length}文字");
            Console.WriteLine();
        }

        Console.Error.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static string Generate(Generator generator, Tokenizer tokenizer)
    {
        var sb = new StringBuilder();
        using var tokenizerStream = tokenizer.CreateStream();
        while (!generator.IsDone())
        {
            try
            {
                generator.ComputeLogits();
                generator.GenerateNextToken();

                var outputTokens = generator.GetSequence(0)[^1];
                var output = tokenizerStream.Decode(outputTokens);

                //Console.Write(output);
                sb.Append(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                break;
            }
        }
        return sb.ToString();
        //Console.WriteLine(sb.ToString());
        //Console.WriteLine();
    }
}

