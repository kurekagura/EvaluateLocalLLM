# README

```console
py -3.12 -m venv .py312
```

[llama-cpp-python](https://github.com/abetlen/llama-cpp-python)

```console
pip install llama-cpp-python --extra-index-url https://abetlen.github.io/llama-cpp-python/whl/cpu

```

モデルを[Hugging Face](https://huggingface.co/elyza/Llama-3-ELYZA-JP-8B-GGUF/tree/main)からダウンロードして、../.modelsフォルダの下におく。

Llama-3-ELYZA-JP-8B-q4_k_m.gguf　約4.58 GB

[READMEに用法の記載がある](https://huggingface.co/elyza/Llama-3-ELYZA-JP-8B-GGUF/blob/main/README.md)

llama-serverを利用すればopenaiと互換があるようだ。

## 参考・謝辞

- [日本語のローカルLLM（文章生成AI）をWindowsで動かす](https://qiita.com/kenta1984/items/7233f8ec9d256f4fa4f7)
