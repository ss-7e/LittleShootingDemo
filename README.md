# LittleShootingDemo

> GameFeelFocusTPSGameDemo  
> 一个聚焦“手感（Game Feel）”的第三人称射击（TPS）游戏 Demo。

## 📌 项目简介

`LittleShootingDemo` 是一个以 **射击手感调优** 为核心目标的实验性 Demo。  
项目重点不在完整商业流程，而在于通过角色控制、镜头反馈、命中表现、特效与着色器等手段，快速验证并迭代 TPS 的核心体验。

## ✨ 核心特性（规划/已实现）

- 第三人称角色控制与基础移动
- 瞄准与射击流程（基础武器交互）
- 命中反馈（Hit Feedback）与视觉表现
- 相机/粒子/材质/后处理驱动的打击感强化

## 🧱 技术栈

根据仓库语言构成：

- **ShaderLab** (~54.9%)：材质与渲染表现核心
- **C#** (~34.5%)：游戏逻辑、角色控制、交互系统
- **HLSL** (~10.6%)：Shader 细节与效果实现

## 🎯 项目目标

- 构建一个轻量、可迭代的 TPS 手感验证场景
- 沉淀“输入 -> 反馈 -> 感知”的调参方法
- 为后续扩展（武器系统、敌人 AI）提供基础框架

## 🚀 快速开始

> 以下步骤可按你当前项目实际情况调整（例如 Unity 版本、输入系统等）

1. 克隆仓库
   ```bash
   git clone https://github.com/ss-7e/LittleShootingDemo.git
   ```
2. 使用 Unity Hub 打开项目目录
3. 等待依赖导入完成
4. 打开主场景（ `Assets/Scenes` 下的 SampleScene 场景）
5. 点击 Play 运行

## 🎮 基础操作（示例）

- `W / A / S / D`：移动
- `Mouse`：视角控制
- `Right Mouse`：瞄准（计划）
- `Left Mouse`：射击
- `Shift`：冲刺（计划）
-  `esc`：暂停

## 📂 目录

```text
LittleShootingDemo/
├─ Assets/
│  ├─ Scripts/          # C# 逻辑代码
│  ├─ Art/              # 所有画面素材
│  │  ├─ Materials/     # 材质资源
│  │  ├─ Animations/    # 动画
│  │  ├─ Shaders/       # shader/hlsl
│  │  ├─ Sound/         # 声音资源
│  │  ├─ Texture/       # 贴图资源
│  ├─ Prefab/           # 预制体
│  ├─ Settings/         # URP/光照设置
│  └─ Scenes/           # 场景
└─ README.md
```

## 🧪 开发重点：Game Feel 清单

你可以围绕以下维度持续迭代：

- 输入响应延迟（开火、转向、瞄准切换）
- 后坐力/镜头震动（强度、恢复速度）
- 命中特效（火花、Decal、受击闪白）
- 音画同步（射击音效与 muzzle flash 对齐）
- 子弹反馈（曳光、UI）

## 🤝 贡献

欢迎提交 Issue / PR 来一起优化手感与表现：

1. Fork 本仓库
2. 新建功能分支：`feature/xxx`
3. 提交修改并说明目的
4. 发起 Pull Request

## 📄 License

All Rights Reserved