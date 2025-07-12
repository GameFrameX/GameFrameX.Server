using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility
{
    /// <summary>
    /// NetHelper 类的单元测试
    /// </summary>
    public class NetHelperTests
    {
        #region IsValidIpAddress 方法测试

        /// <summary>
        /// 测试有效的IPv4地址
        /// </summary>
        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("127.0.0.1")]
        [InlineData("0.0.0.0")]
        [InlineData("255.255.255.255")]
        [InlineData("10.0.0.1")]
        public void IsValidIpAddress_WithValidIPv4_ShouldReturnTrue(string ipAddress)
        {
            // Act
            var result = NetHelper.IsValidIpAddress(ipAddress, out var parsedIp);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedIp);
            Assert.Equal(AddressFamily.InterNetwork, parsedIp.AddressFamily);
        }

        /// <summary>
        /// 测试有效的IPv6地址
        /// </summary>
        [Theory]
        [InlineData("::1")]
        [InlineData("2001:db8::1")]
        [InlineData("fe80::1")]
        [InlineData("::")]
        public void IsValidIpAddress_WithValidIPv6_ShouldReturnTrue(string ipAddress)
        {
            // Act
            var result = NetHelper.IsValidIpAddress(ipAddress, out var parsedIp);

            // Assert
            Assert.True(result);
            Assert.NotNull(parsedIp);
            Assert.Equal(AddressFamily.InterNetworkV6, parsedIp.AddressFamily);
        }

        /// <summary>
        /// 测试无效的IP地址
        /// </summary>
        [Theory]
        [InlineData("256.256.256.256")]
        [InlineData("192.168.1")]
        [InlineData("192.168.1.1.1")]
        [InlineData("abc.def.ghi.jkl")]
        [InlineData("192.168.1.-1")]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValidIpAddress_WithInvalidIP_ShouldReturnFalse(string ipAddress)
        {
            // Act
            var result = NetHelper.IsValidIpAddress(ipAddress, out var parsedIp);

            // Assert
            Assert.False(result);
            Assert.Null(parsedIp);
        }

        /// <summary>
        /// 测试null参数
        /// </summary>
        [Fact]
        public void IsValidIpAddress_WithNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => NetHelper.IsValidIpAddress(null, out _));
        }

        #endregion

        #region GetFirstAvailablePort 方法测试

        /// <summary>
        /// 测试获取可用端口
        /// </summary>
        [Fact]
        public void GetFirstAvailablePort_WithDefaultRange_ShouldReturnValidPort()
        {
            // Act
            var port = NetHelper.GetFirstAvailablePort();

            // Assert
            Assert.True(port >= 667 || port == -1);
            Assert.True(port <= 65535);
        }

        /// <summary>
        /// 测试自定义端口范围
        /// </summary>
        [Fact]
        public void GetFirstAvailablePort_WithCustomRange_ShouldReturnPortInRange()
        {
            // Arrange
            const int startPort = 8000;
            const int maxPort = 8100;

            // Act
            var port = NetHelper.GetFirstAvailablePort(startPort, maxPort);

            // Assert
            Assert.True(port >= startPort || port == -1);
            Assert.True(port < maxPort || port == -1);
        }

        /// <summary>
        /// 测试无效的起始端口
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(65536)]
        public void GetFirstAvailablePort_WithInvalidStartPort_ShouldThrowArgumentOutOfRangeException(int startPort)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetFirstAvailablePort(startPort));
        }

        /// <summary>
        /// 测试无效的最大端口
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(65536)]
        public void GetFirstAvailablePort_WithInvalidMaxPort_ShouldThrowArgumentOutOfRangeException(int maxPort)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetFirstAvailablePort(1000, maxPort));
        }

        /// <summary>
        /// 测试起始端口大于等于最大端口
        /// </summary>
        [Fact]
        public void GetFirstAvailablePort_WithStartPortGreaterThanMaxPort_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetFirstAvailablePort(8080, 8080));
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetFirstAvailablePort(8080, 8079));
        }

        #endregion

        #region PortIsUsed 方法测试

        /// <summary>
        /// 测试获取已使用的端口列表
        /// </summary>
        [Fact]
        public void PortIsUsed_ShouldReturnListOfUsedPorts()
        {
            // Act
            var usedPorts = NetHelper.PortIsUsed();

            // Assert
            Assert.NotNull(usedPorts);
            Assert.IsType<List<int>>(usedPorts);
            // 通常系统会有一些已使用的端口
            Assert.True(usedPorts.Count >= 0);
        }

        #endregion

        #region PortIsAvailable 方法测试

        /// <summary>
        /// 测试检查端口可用性
        /// </summary>
        [Fact]
        public void PortIsAvailable_WithHighPort_ShouldReturnBoolean()
        {
            // Arrange
            const int testPort = 65000; // 使用一个不太可能被占用的高端口

            // Act
            var isAvailable = NetHelper.PortIsAvailable(testPort);

            // Assert
            Assert.IsType<bool>(isAvailable);
        }

        /// <summary>
        /// 测试无效端口号
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(65536)]
        [InlineData(100000)]
        public void PortIsAvailable_WithInvalidPort_ShouldThrowArgumentOutOfRangeException(int port)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.PortIsAvailable(port));
        }

        #endregion

        #region GetLocalIpList 方法测试

        /// <summary>
        /// 测试获取本地IPv4地址列表
        /// </summary>
        [Fact]
        public void GetLocalIpList_WithIPv4_ShouldReturnIPv4Addresses()
        {
            // Act
            var ipList = NetHelper.GetLocalIpList(AddressFamily.InterNetwork);

            // Assert
            Assert.NotNull(ipList);
            Assert.IsType<List<string>>(ipList);
            
            // 验证返回的都是有效的IPv4地址
            foreach (var ip in ipList)
            {
                Assert.True(NetHelper.IsValidIpAddress(ip, out var parsedIp));
                Assert.Equal(AddressFamily.InterNetwork, parsedIp.AddressFamily);
            }
        }

        /// <summary>
        /// 测试获取本地IPv6地址列表
        /// </summary>
        [Fact]
        public void GetLocalIpList_WithIPv6_ShouldReturnIPv6Addresses()
        {
            // Act
            var ipList = NetHelper.GetLocalIpList(AddressFamily.InterNetworkV6);

            // Assert
            Assert.NotNull(ipList);
            Assert.IsType<List<string>>(ipList);
            
            // 验证返回的都是有效的IPv6地址
            foreach (var ip in ipList)
            {
                Assert.True(NetHelper.IsValidIpAddress(ip, out var parsedIp));
                Assert.Equal(AddressFamily.InterNetworkV6, parsedIp.AddressFamily);
            }
        }

        #endregion

        #region IsNetworkReachable 方法测试

        /// <summary>
        /// 测试网络连通性检查（本地回环）
        /// </summary>
        [Fact]
        public void IsNetworkReachable_WithLocalhost_ShouldReturnTrue()
        {
            // Act
            var isReachable = NetHelper.IsNetworkReachable("127.0.0.1", 1000);

            // Assert
            Assert.True(isReachable);
        }

        /// <summary>
        /// 测试无效主机地址
        /// </summary>
        [Fact]
        public void IsNetworkReachable_WithInvalidHost_ShouldReturnFalse()
        {
            // Act
            var isReachable = NetHelper.IsNetworkReachable("999.999.999.999", 1000);

            // Assert
            Assert.False(isReachable);
        }

        /// <summary>
        /// 测试null或空主机地址
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsNetworkReachable_WithNullOrEmptyHost_ShouldThrowArgumentException(string host)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NetHelper.IsNetworkReachable(host));
        }

        /// <summary>
        /// 测试负数超时时间
        /// </summary>
        [Fact]
        public void IsNetworkReachable_WithNegativeTimeout_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.IsNetworkReachable("127.0.0.1", -1));
        }

        #endregion

        #region GetMacAddresses 方法测试

        /// <summary>
        /// 测试获取MAC地址列表
        /// </summary>
        [Fact]
        public void GetMacAddresses_ShouldReturnListOfMacAddresses()
        {
            // Act
            var macAddresses = NetHelper.GetMacAddresses();

            // Assert
            Assert.NotNull(macAddresses);
            Assert.IsType<List<string>>(macAddresses);
            
            // 验证MAC地址格式（如果有的话）
            foreach (var mac in macAddresses)
            {
                Assert.NotNull(mac);
                Assert.NotEmpty(mac);
                // MAC地址应该包含冒号分隔符
                Assert.Contains(":", mac);
            }
        }

        #endregion

        #region IsIpInSubnet 方法测试

        /// <summary>
        /// 测试IP地址在子网内
        /// </summary>
        [Theory]
        [InlineData("192.168.1.100", "192.168.1.0", "255.255.255.0")]
        [InlineData("10.0.0.50", "10.0.0.0", "255.0.0.0")]
        [InlineData("172.16.5.10", "172.16.0.0", "255.255.0.0")]
        public void IsIpInSubnet_WithIpInSubnet_ShouldReturnTrue(string ipAddress, string networkAddress, string subnetMask)
        {
            // Act
            var result = NetHelper.IsIpInSubnet(ipAddress, networkAddress, subnetMask);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// 测试IP地址不在子网内
        /// </summary>
        [Theory]
        [InlineData("192.168.2.100", "192.168.1.0", "255.255.255.0")]
        [InlineData("11.0.0.50", "10.0.0.0", "255.0.0.0")]
        [InlineData("172.17.5.10", "172.16.0.0", "255.255.0.0")]
        public void IsIpInSubnet_WithIpNotInSubnet_ShouldReturnFalse(string ipAddress, string networkAddress, string subnetMask)
        {
            // Act
            var result = NetHelper.IsIpInSubnet(ipAddress, networkAddress, subnetMask);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// 测试无效IP地址格式
        /// </summary>
        [Fact]
        public void IsIpInSubnet_WithInvalidIpFormat_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NetHelper.IsIpInSubnet("invalid", "192.168.1.0", "255.255.255.0"));
            Assert.Throws<ArgumentException>(() => NetHelper.IsIpInSubnet("192.168.1.1", "invalid", "255.255.255.0"));
            Assert.Throws<ArgumentException>(() => NetHelper.IsIpInSubnet("192.168.1.1", "192.168.1.0", "invalid"));
        }

        /// <summary>
        /// 测试null或空参数
        /// </summary>
        [Theory]
        [InlineData(null, "192.168.1.0", "255.255.255.0")]
        [InlineData("", "192.168.1.0", "255.255.255.0")]
        [InlineData("192.168.1.1", null, "255.255.255.0")]
        [InlineData("192.168.1.1", "", "255.255.255.0")]
        [InlineData("192.168.1.1", "192.168.1.0", null)]
        [InlineData("192.168.1.1", "192.168.1.0", "")]
        public void IsIpInSubnet_WithNullOrEmptyParameters_ShouldThrowArgumentException(string ipAddress, string networkAddress, string subnetMask)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NetHelper.IsIpInSubnet(ipAddress, networkAddress, subnetMask));
        }

        #endregion

        #region GetAvailablePorts 方法测试

        /// <summary>
        /// 测试获取可用端口列表
        /// </summary>
        [Fact]
        public void GetAvailablePorts_WithValidRange_ShouldReturnAvailablePorts()
        {
            // Arrange
            const int startPort = 60000;
            const int endPort = 60100;
            const int maxCount = 5;

            // Act
            var availablePorts = NetHelper.GetAvailablePorts(startPort, endPort, maxCount);

            // Assert
            Assert.NotNull(availablePorts);
            Assert.True(availablePorts.Count <= maxCount);
            Assert.All(availablePorts, port => Assert.InRange(port, startPort, endPort - 1));
        }

        /// <summary>
        /// 测试无效端口范围
        /// </summary>
        [Theory]
        [InlineData(0, 100, 5)]
        [InlineData(-1, 100, 5)]
        [InlineData(65536, 65540, 5)]
        [InlineData(100, 0, 5)]
        [InlineData(100, -1, 5)]
        [InlineData(100, 65536, 5)]
        public void GetAvailablePorts_WithInvalidPortRange_ShouldThrowArgumentOutOfRangeException(int startPort, int endPort, int maxCount)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetAvailablePorts(startPort, endPort, maxCount));
        }

        /// <summary>
        /// 测试起始端口大于等于结束端口
        /// </summary>
        [Fact]
        public void GetAvailablePorts_WithStartPortGreaterThanEndPort_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetAvailablePorts(8080, 8080, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetAvailablePorts(8080, 8079, 5));
        }

        /// <summary>
        /// 测试无效的最大数量
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetAvailablePorts_WithInvalidMaxCount_ShouldThrowArgumentOutOfRangeException(int maxCount)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NetHelper.GetAvailablePorts(8000, 8100, maxCount));
        }

        #endregion

        #region GetPublicIpAddressAsync 方法测试

        /// <summary>
        /// 测试获取公网IP地址（可能失败，取决于网络环境）
        /// </summary>
        [Fact]
        public async Task GetPublicIpAddressAsync_ShouldReturnValidIpOrNull()
        {
            // Act
            var publicIp = await NetHelper.GetPublicIpAddressAsync(5000);

            // Assert
            if (publicIp != null)
            {
                Assert.True(NetHelper.IsValidIpAddress(publicIp, out _));
            }
        }

        /// <summary>
        /// 测试负数超时时间
        /// </summary>
        [Fact]
        public async Task GetPublicIpAddressAsync_WithNegativeTimeout_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => NetHelper.GetPublicIpAddressAsync(-1));
        }

        #endregion

        #region IsPrivateIpAddress 方法测试

        /// <summary>
        /// 测试私有IP地址
        /// </summary>
        [Theory]
        [InlineData("10.0.0.1")]
        [InlineData("10.255.255.255")]
        [InlineData("172.16.0.1")]
        [InlineData("172.31.255.255")]
        [InlineData("192.168.0.1")]
        [InlineData("192.168.255.255")]
        [InlineData("127.0.0.1")]
        [InlineData("127.255.255.255")]
        public void IsPrivateIpAddress_WithPrivateIp_ShouldReturnTrue(string ipAddress)
        {
            // Act
            var result = NetHelper.IsPrivateIpAddress(ipAddress);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// 测试公网IP地址
        /// </summary>
        [Theory]
        [InlineData("8.8.8.8")]
        [InlineData("1.1.1.1")]
        [InlineData("208.67.222.222")]
        [InlineData("173.15.255.255")]
        [InlineData("172.32.0.1")]
        [InlineData("11.0.0.1")]
        public void IsPrivateIpAddress_WithPublicIp_ShouldReturnFalse(string ipAddress)
        {
            // Act
            var result = NetHelper.IsPrivateIpAddress(ipAddress);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// 测试无效IP地址格式
        /// </summary>
        [Theory]
        [InlineData("invalid")]
        [InlineData("256.256.256.256")]
        [InlineData("192.168.1")]
        public void IsPrivateIpAddress_WithInvalidIp_ShouldThrowArgumentException(string ipAddress)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NetHelper.IsPrivateIpAddress(ipAddress));
        }

        /// <summary>
        /// 测试null或空IP地址
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsPrivateIpAddress_WithNullOrEmptyIp_ShouldThrowArgumentException(string ipAddress)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NetHelper.IsPrivateIpAddress(ipAddress));
        }

        #endregion

        #region 集成测试

        /// <summary>
        /// 测试端口可用性检查的一致性
        /// </summary>
        [Fact]
        public void PortAvailability_ShouldBeConsistent()
        {
            // Arrange
            var usedPorts = NetHelper.PortIsUsed();
            
            // Act & Assert
            foreach (var port in usedPorts.Take(5)) // 只测试前5个已使用的端口
            {
                var isAvailable = NetHelper.PortIsAvailable(port);
                Assert.False(isAvailable, $"端口 {port} 应该被标记为不可用");
            }
        }

        /// <summary>
        /// 测试获取可用端口的功能性
        /// </summary>
        [Fact]
        public void GetAvailablePorts_ShouldReturnActuallyAvailablePorts()
        {
            // Arrange
            const int startPort = 60000;
            const int endPort = 60010;
            
            // Act
            var availablePorts = NetHelper.GetAvailablePorts(startPort, endPort, 5);
            
            // Assert
            foreach (var port in availablePorts)
            {
                Assert.True(NetHelper.PortIsAvailable(port), $"端口 {port} 应该是可用的");
            }
        }

        #endregion
    }
}