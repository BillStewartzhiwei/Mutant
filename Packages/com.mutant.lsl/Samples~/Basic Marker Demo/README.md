# Basic Marker Demo

这个示例演示：

- 创建 `Mutant.Marker` sender
- 发送 marker
- 接收 `Mutant.Marker` stream

## 使用方法

1. 将 Sample 导入到 Unity 工程。
2. 创建两个空物体：
    - `MarkerSender`
    - `MarkerReceiver`
3. 分别挂载：
    - `MutantLslBasicMarkerSenderSample`
    - `MutantLslBasicMarkerReceiverSample`
4. 运行场景。
5. 按下 `Space`，发送一个 marker。
6. 在 Console 中查看接收结果。