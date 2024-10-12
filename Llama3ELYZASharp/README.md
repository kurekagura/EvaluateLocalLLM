# LLamaSharp

[LLamaSharp Documentation](https://scisharp.github.io/LLamaSharp/0.16.0/QuickStart/)


����p�̃A�Z�b�g�𗘗p���Ă����B
[Chinese LLM - with GB2312 encoding](https://scisharp.github.io/LLamaSharp/0.16.0/Examples/ChatChineseGB2312/)


## �l��ނ�Executor

1. InteractiveExecutor
1. InstructExecutor

    �u�������݂𑱂���v�Ȃǂ̎w�������s������B

1. StatelessExecutor

    ������̃W���u�ɓK���Ă���B

1. BatchedExecutor

    �����̓��͂��󂯕t���A�قȂ�Z�b�V�����̕����̏o�͂𓯎��ɐ������邱�Ƃ��ł���B

## ���\����

```xml
  <ItemGroup>
    <PackageReference Include="LLamaSharp" Version="0.16.0" />
    <PackageReference Include="LLamaSharp.Backend.Cpu" Version="0.16.0" />
  </ItemGroup>
```

>���v����: 86.91 �b �v�񕶎����F1636 -> 181����


```xml
  <ItemGroup>
    <PackageReference Include="LLamaSharp" Version="0.16.0" />
    <PackageReference Include="LLamaSharp.Backend.Cuda11" Version="0.16.0" />
  </ItemGroup>
```

>���v����: 19.84 �b �v�񕶎����F1636 -> 211����