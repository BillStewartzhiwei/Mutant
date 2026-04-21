# Mutant Core

Mutant Core 是 Mutant 框架的基础包，提供：

- CoreBootstrap
- CoreSettings
- ModuleManager
- IModule / ModuleBase
- EventBus / StickyEventBus

## 功能

- 模块注册与生命周期管理
- 可选自动扫描注册
- 普通事件总线
- 粘性事件总线
- Bootstrap 启动入口

## 安装

使用Upm进行GitURL安装：
https://github.com/BillStewartzhiwei/Mutant.git?path=/Packages/com.mutant.core#main

将本包放到：

Packages/com.mutant.core

Unity 会将它识别为本地 UPM 包。

## 快速开始

1. 在场景中创建空物体 `[MutantCore]`
2. 挂载 `CoreBootstrap`
3. 创建一个 `CoreSettings` 资源并拖给 `CoreBootstrap`
4. 注册你的模块，或启用自动扫描
5. 运行场景

## Sample

Package Manager 中导入 `Basic Usage` sample。