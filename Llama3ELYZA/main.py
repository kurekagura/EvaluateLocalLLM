import time
from llama_cpp import Llama


def load_model():
    llm = Llama(
        model_path="../.models/Llama-3-ELYZA-JP-8B-q4_k_m.gguf",
        chat_format="llama-3",
        n_ctx=2048,
    )
    return llm


def create_summary(llm: Llama, userMessage: str):
    response = llm.create_chat_completion(
        messages=[
            {
                "role": "system",
                "content": "あなたは日本語の文章の専門家です。",
            },
            {
                "role": "user",
                "content": f"以下の文章を512文字程度に要約せよ。要約のみを示せ。\r\n{userMessage}",
            },
        ],
        max_tokens=1024,
    )
    return response


if __name__ == "__main__":

    with open("../../PrivateJunkData/20241010_UserMessageList.txt", "r", encoding="utf-8") as file:
        content = file.read()

    # 空行ごとに分割してリストに格納
    userMessageList = content.split("\n\n")

    llm = load_model()
    # 各メッセージをループで表示
    for index, userMessage in enumerate(userMessageList):
        print(f"文章-{index + 1}:")
        start_time = time.time()
        response = create_summary(llm, userMessage)
        end_time = time.time()
        content = response["choices"][0]["message"]["content"]
        print(content)
        elapsed_time = end_time - start_time
        print(f"所要時間: {elapsed_time:.2f} 秒 要約文字数：{len(userMessage)} -> {len(content)}文字")
        print("-" * 20)
