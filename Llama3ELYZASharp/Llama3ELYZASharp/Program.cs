using LLama.Common;
using LLama;
using LLama.Abstractions;
using System.Text;
using System.Diagnostics;

namespace Llama3ELYZASharp;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var sw = new Stopwatch();
        string modelFilePath = @"O:\\models\Llama\Llama-3-ELYZA-JP-8B-q4_k_m.gguf";

        try
        {
            //EncodingをUTF8に設定すると精度が良くなった気がする
            var parameters = new ModelParams(modelFilePath)
            {
                ContextSize = 2048,
                //GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
                Seed = 1337,
                Encoding = Encoding.UTF8
            };



            sw.Start();
            //model/contextをdisposeするとexecutorが壊れる（関数化する場合、注意）
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            sw.Stop();

            //var executor = new InteractiveExecutor(context);
            //var executor = new InstructExecutor(context);
            //var executor = new StatelessExecutor(model,context.Params);

            Console.WriteLine($"モデルロード所要時間： {sw.Elapsed.TotalSeconds:F2} 秒");

            var userMessageList = new List<string>(File.ReadAllText(@"../../../PrivateJunkData/20241010_UserMessageList.txt").Split("\r\n\r\n", StringSplitOptions.None));

            foreach (var (userMessage, index) in userMessageList.Select((message, i) => (message, i)))
            {
                //var executor = new InteractiveExecutor(context);
                //var executor = new InstructExecutor(context);
                var executor = new StatelessExecutor(model, context.Params);

                Console.WriteLine($"問い合わせ開始・・・");

                sw.Start();
                //var response = await ChatSummary(executor, userMessage);
                //var response = await InferSummary(executor, userMessage);
                var response = await InferSummary(executor, userMessage);
                sw.Stop();

                Console.WriteLine($"回答（要約）ー {index + 1}:");
                Console.WriteLine(response);
                Console.WriteLine($"所要時間: {sw.Elapsed.TotalSeconds:F2} 秒 要約文字数：{userMessage.Length} -> {response.Length}文字");
                Console.WriteLine(new string('-', 20));
            }
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return -1;
        }
        finally
        {
            Console.Error.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    //文字でないものが末尾に付く。AntiPromptsの使い方が関係している？
    static async Task<string> ChatSummary(InteractiveExecutor executor, string userMessage)
    {
        var sb = new StringBuilder();
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "あなたは日本語の文章の専門家です。");
        //Userを２回続けると例外発生する
        var session = new ChatSession(executor, chatHistory);

        var inferenceParams = new InferenceParams()
        {
            //MaxTokens個を超えるトークンがアンサーに現れてはならない。アンチプロンプトで十分な場合は不要。
            //MaxTokens = 4096, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
            AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
        };

        //streaminglyなAPIのみ？
        var ienum = session.ChatAsync(new ChatHistory.Message(AuthorRole.User, $"次の文章を512文字程度に要約してください。{Environment.NewLine}{userMessage}"), inferenceParams);
        await foreach (var text in ienum)
        {
            sb.Append(text);
        }
        return sb.ToString();
    }

    static async Task<string> InferSummary(InstructExecutor executor, string userMessage)
    {
        var sb = new StringBuilder();

        var inferenceParams = new InferenceParams()
        {
            //MaxTokens個を超えるトークンがアンサーに現れてはならない。アンチプロンプトで十分な場合は不要。
            MaxTokens = 2048, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
                              //AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
        };

        var ienum = executor.InferAsync($"次の文章を512文字程度に要約してください。{Environment.NewLine}###{Environment.NewLine}{userMessage}", inferenceParams);
        await foreach (var text in ienum)
        {
            sb.Append(text);
        }
        return sb.ToString();
    }

    //これが一番よさそう。
    static async Task<string> InferSummary(StatelessExecutor executor, string userMessage)
    {
        var sb = new StringBuilder();

        var inferenceParams = new InferenceParams()
        {
            //MaxTokens個を超えるトークンがアンサーに現れてはならない。アンチプロンプトで十分な場合は不要。
            MaxTokens = 2048, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
                              //AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
        };

        var ienum = executor.InferAsync($"次の文章を512文字程度に要約して結果のみを示してください。要約に改行は不要です。回答はJSONフォーマットで、キーは\"summary\"を利用してください。{Environment.NewLine}###{Environment.NewLine}{userMessage}");
        await foreach (var text in ienum)
        {
            sb.Append(text);
        }
        return sb.ToString();
    }

}
