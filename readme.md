

### [Unity3d中UGUI图文混排方案](https://github.com/lixianmin/richtext/blob/master/readme.md)

---
#### 设计目的

1. 在同一个material中支持图文混排，这样可以有效的降低draw call数目
2. 支持血条：当场景中人物数量较多的时候，由于人物移动相对频繁，血条的draw call数以及由此引起的panel rebuild可能会成为性能瓶颈

---
#### 测试案例

1. 大规模图文混排测试：
	1. 可以看到，当图文在一个material中时，draw call数还是很低的；
	2. 当使用到不同material中的texture时，可以考虑将它们放置到同一个canvas下的不同子节点下面，这样不同的子节点（含子节点下的图文结点）之间的绘制顺序由transform的顺序保证；

使用到的测试用例，结点结构如下：

![DrawCall测试布局](https://raw.githubusercontent.com/lixianmin/richtext/master/readme/images/drawcall-layout.jpg)

 从图中也可以看到，水果图标的节点总是在小黑猫（默认图标）节点的后面绘制，从而在结构上保证了drawcall数较低。

![DrawCall测试](https://raw.githubusercontent.com/lixianmin/richtext/master/readme/images/drawcall-test.jpg)


2. 血条测试用例：

使用的测试文本如下：

```
<quad name=foreground src=emoji/bloodbar width=96 height=12 />
<color=lime>这里是头顶的字</color>
<color=#ffa500ff>颜色支持#ffa500ff格式，支持透明度</color>
<b>这一行字是粗的</b>
<i>这一行字是斜的</i>
<size=24>这一行字是大的</size>
```
效果如图：

![头顶血条测试](https://raw.githubusercontent.com/lixianmin/richtext/master/readme/images/bloodbar-test.jpg)

---
#### 测试平台

mac os + Unity3d 2017.1.0f3 (64bit)

由于unity3d的场景文件(.unity)在不同的版本间结构改动很大，因此使用其它版本的unity3d打开测试工程的时候可能会打不开。

代码本身最早设计是在Unity3d 5.4.3p1中进行的，因此从代码本身的兼容性上，从5.4 ~ 2017.01应该是没有问题的

---
#### references

1. [Unity3D游戏优化之头顶UI](https://zhuanlan.zhihu.com/p/25670078)
2. [RichText](https://docs.unity3d.com/Manual/StyledText.html)
3. [Unity-EmojiText](https://github.com/carlosCn/Unity-EmojiText)

