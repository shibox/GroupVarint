# GroupVarint
the fast groupvarint implement for .net


## 使用方法
Usage is very simple:
```

编码
GroupVarintCodec.Encode(buffer, 0, array, 0, array.Length);
buffer：写入的二进制缓冲区，注意缓冲区大小为要压缩的元素的5倍，否则可能出现越界的异常
array：需要压缩的int数据，注意这个数组的数据集大小必须是4的整数倍

解码
GroupVarintCodec.Decode(buffer, offset, count, dst, ref apos);
buffer：二进制数据缓冲区
offset：从二进制缓冲区解码的起始位置
count：需要解码的二进制数据的字节数量
dst：解压到的int缓冲区
apos：解码后dst数据的指向

```

## Nuget Package
This code is available as the Nuget package [`HyperLogLog`](https://www.nuget.org/packages/GroupVarint/).  To install, run the following command in the Package Manager Console:

```
Install-Package GroupVarint
```
运行配置CPU：E3-1231 v3 3.4GHz 内存：16G
### 编码性能： 100,000,000 元素编码最快只需366毫秒，接近每秒编码2.7 亿uint数据，接近每秒压缩1.1G的二进制数据
### 解码性能： 100,000,000 元素编码最快只需110毫秒，接近每秒解码：9亿uint数据，接近每秒解压3.6G的二进制数据



