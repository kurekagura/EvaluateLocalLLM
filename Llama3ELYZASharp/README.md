# LLamaSharp

[LLamaSharp Documentation](https://scisharp.github.io/LLamaSharp/0.16.0/QuickStart/)


言語用のアセットを利用している例。
[Chinese LLM - with GB2312 encoding](https://scisharp.github.io/LLamaSharp/0.16.0/Examples/ChatChineseGB2312/)


## 四種類のExecutor

1. InteractiveExecutor
1. InstructExecutor

    「書き込みを続けろ」などの指示を実行させる。

1. StatelessExecutor

    一回限りのジョブに適している。

1. BatchedExecutor

    複数の入力を受け付け、異なるセッションの複数の出力を同時に生成することができる。

## 性能メモ

```xml
  <ItemGroup>
    <PackageReference Include="LLamaSharp" Version="0.16.0" />
    <PackageReference Include="LLamaSharp.Backend.Cpu" Version="0.16.0" />
  </ItemGroup>
```

>所要時間: 86.91 秒 要約文字数：1636 -> 181文字


```xml
  <ItemGroup>
    <PackageReference Include="LLamaSharp" Version="0.16.0" />
    <PackageReference Include="LLamaSharp.Backend.Cuda11" Version="0.16.0" />
  </ItemGroup>
```

>所要時間: 19.84 秒 要約文字数：1636 -> 211文字