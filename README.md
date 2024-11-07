# Shoting

学到了很多 [DEMO](https://github.com/SwimmingBotA/Shoting/issues/1)

# 较为完整的做了个游戏流程

收获很多，不管是在面向对象上的用法加深，还是在游戏设计上的理解
这个项目全程使用协程，没有使用Update，在引入Unity新出的Input System之后，完全可以让用户自定义去指定事件的委托



Unity新推出的New Input System很好的解决了按键冲突的问题，虽然旧版Input Manager中也可以做各个键的设置，但是这种由自己定制的理解一套流程，也是一种不错的经验

Input System界面中得Action Map用来解决按键冲突，如Ui界面时，我们也需要WSAD键来操作，人物移动我也是WSAD，这就冲突了，这时候我们只要警用Player的Action Map，启用UI的Action Map即可

当完成Action map与Action的配置后，令Input System生成一个脚本文件，我们要用到的此脚本里面的OnMove，它是用来检测我们有无键盘输入的，它属于IGamePlayerActions接口，所以得先继承我们这个接口先，为了以后方便其他类调用这个脚本，继承ScriptableObject作为一个脚本文件

将Input System生成的脚本文件实例化，并调用其中的GamePlayer.SetCallbacks，将我们此时这个类回调回去，之后就可以调用GamePlayer.Enable()与GamePlayer.Disable()让其启用禁用动作表了

输入肯定得有事件，那就得去查找InputActionPhase这个枚举是什么状态了，Performed相当于GetKey，
Canceled相当于GetKeyUp，Started相当于GetKeyDown，Disabled禁用时，Waiting启用时



# 首先是对象池Pool System的学习

### 对象池（Objec Pool）模式（优化型）
常用子弹、敌人、特效等频繁出现的物件
若在通常流程里，我们生成一个预制体，一般会克隆它，也就是Instantiate，这个过程是在我们内存空间中分配一块空间给我们物件存储，然后在合适时间被GC回收，一切看起来都那么美好，首先GC并非及时的，且很难去控制，而且当我们这个预制体生成的过快过多时候，产生GC也会消耗大量CPU时间，这就让我们游戏卡顿直至停止运行
`所以`要引入对象池模式
优点：
- 可重复利用
- 不会触发GC
- 消耗性能少

对象池，在游戏初始化阶段就完成空间的分配，这款空间不会被GC，也就不会消耗大量CPU性能，且不必在new新对象了，前提你得做好你的预制体的生命周期或者你池子的回收条件

如何做好一个对象池思路
以武器子弹举例，这个脚本必须得能兼容我们也会各种子弹的初始化吧
- 也就是生成预备好的对象，一般我们会用队列Queue类型去存储要的个数，一个队列里加入我们预备克隆好的GameObject。
- 然后就是去Pool池子里取它，有可用对象就让它出列，没有可用对象就克隆一个，所以要先Debug我们要的池子大小，避免不必要的问题
- 取得之后释放它，最后回收时候将其加入队列，回收可以在出列时加入队列

造好了池子了，但是这只是一个的，所以我们得引入一个数组去存它，这里推荐字典，利用我们要的子弹作为键，这个子弹所属的池子作为值，这就很好的构成一个对应关系

最后要注意的预制体必须是对应存在的，不能说你拿个这个字典中不存在的键去找值吧

### 其次是对于协程（Coroutine）的场合选择

如果说有什么可以替代Update，那么肯定是协程的运用，一个好的程序设计考虑的是性能的损耗，对于一直处于调用状态的Update，或多或少都会影响性能优劣，这也可以看出新的Input System的重要性

### 然后是UI制作与异步加载

UI的渲染也会造成性能上的偏差，如静态UI与动态UI，Unity官方推荐我们选择静态画布来存储静态UI，动态画布来存储动态UI，这也静态就只渲染一次，从而减低损耗

那么在加载时候，加入异步加载[SceneManager.LoadSceneAsync](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html "SceneManager.LoadSceneAsync")
以及它所返回的类[AsyncOperation](https://docs.unity3d.com/ScriptReference/AsyncOperation.html "AsyncOperation")
它能很好解决LoadScene发生的卡顿，和提高性能

### 如何监控游戏中的敌人

将对象池中的敌人释放出来，加入列表中，这样就可以完成敌人的监控了，而yield return new WaitUntil则是协程的事件等待

### 说到监控，就不得不说在游戏设计中的各种Manager了

GameManager管理游戏状态，首先有且只有一个，是个单例，这样当玩家做任何操作时，都会通知GameManager，此时处于什么状态，那么在场景中的各个有关联的物件就会做出对应操作
类似MVC模式，但是缺少了Manager对其他的操作

然后还有各种Manager将功能分发，如AudioManager、Scened Manager等
