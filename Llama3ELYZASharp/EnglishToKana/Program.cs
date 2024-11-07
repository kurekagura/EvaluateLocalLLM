using LLama.Common;
using LLama;
using LLama.Abstractions;
using System.Text;
using System.Diagnostics;
using System;
using Microsoft.Extensions.Logging;
using LLama.Native;
using System.Text.Json;
using System.Collections.Generic;

namespace EnglishToKana;

internal class Program
{
    static async Task Main(string[] args)
    {
        string modelFilePath = @"O:\\models\Llama\Llama-3-ELYZA-JP-8B-q4_k_m.gguf";

        try
        {
            var nullLogger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<Program>();
            NativeLibraryConfig.All.WithLogCallback(nullLogger);

            var modelParams = new ModelParams(modelFilePath)
            {
                ContextSize = 2048,
                //GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
                Seed = 1337,
                Encoding = Encoding.UTF8,

            };

            using var model = LLamaWeights.LoadFromFile(modelParams);
            using var context = model.CreateContext(modelParams, nullLogger);

            var userMessageList = new List<string>(File.ReadAllText(@"EnglishToKana.txt").Split("\r\n", StringSplitOptions.RemoveEmptyEntries));
            foreach (var (userMessage, index) in userMessageList.Select((message, i) => (message, i)))
            {
                var executor = new StatelessExecutor(model, context.Params);

                //Console.WriteLine($"問い合わせ開始・・・");

                var sw = new Stopwatch();
                sw.Start();
                var response = await InferEnglishToKana(executor, userMessage);
                sw.Stop();

                Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(response) ?? new();
                foreach (var item in dic)
                {
                    Console.WriteLine($"{item.Key} : {item.Value}");
                }

                Console.WriteLine($"所要時間: {sw.Elapsed.TotalSeconds:F2} 秒 単語数：{dic.Count} {sw.Elapsed.TotalSeconds / dic.Count:F2}秒/単語");
                Console.WriteLine(new string('-', 20));
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    static async Task<string> InferEnglishToKana(StatelessExecutor executor, string userMessage)
    {
        string message = $"キーが英単語、値がカタカナ読みの次のJSONを完成させてください。{userMessage}{Environment.NewLine}回答:";
        var sb = new StringBuilder();

        //var inferenceParams = new InferenceParams()
        //{
        //    //MaxTokens個を超えるトークンがアンサーに現れてはならない。アンチプロンプトで十分な場合は不要。
        //    MaxTokens = 2048, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
        //                      //AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
        //};

        //Console.WriteLine(message);

        var ienum = executor.InferAsync(message);
        await foreach (var text in ienum)
        {
            sb.Append(text);
        }
        return sb.ToString();
    }
}
