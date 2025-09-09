using System;
using System.Collections.Generic;
using System.Linq;

// 图书馆物品类型枚举
public enum MediaType
{
    Novel,          // 小说
    Magazine,       // 杂志
    Textbook,       // 教科书
    ReferenceBook,  // 参考书
    AudioBook       // 有声书
}

// 借阅状态枚举
public enum BorrowStatus
{
    Available,      // 可借阅
    Borrowed,       // 已借出
    Reserved,       // 已预约
    UnderRepair     // 维修中
}

// 图书馆物品基类
public abstract class LibraryItem
{
    // 只读属性
    public int Id { get; }
    public string Title { get; }
    public MediaType MediaType { get; }
    public BorrowStatus Status { get; protected set; }
    
    // 出版信息
    public int PublicationYear { get; }
    
    // 内部计数器
    private static int _nextId = 1;

    // 构造函数
    protected LibraryItem(string title, MediaType mediaType, int publicationYear)
    {
        Id = _nextId++;
        Title = title;
        MediaType = mediaType;
        PublicationYear = publicationYear;
        Status = BorrowStatus.Available;
    }

    // 抽象方法 - 获取详细信息
    public abstract string GetDetails();
    
    // 标记为借出
    public virtual void MarkAsBorrowed()
    {
        if (Status == BorrowStatus.Available)
        {
            Status = BorrowStatus.Borrowed;
        }
        else
        {
            throw new InvalidOperationException("当前状态不可借出");
        }
    }
    
    // 标记为可借阅
    public virtual void MarkAsAvailable()
    {
        Status = BorrowStatus.Available;
    }
    
    // 重写ToString方法
    public override string ToString()
    {
        return $"{Id}: {Title} ({MediaType})";
    }
}

// 小说类
public class Novel : LibraryItem
{
    public string Author { get; }
    public string Genre { get; } // 小说类型（科幻、言情等）

    public Novel(string title, string author, string genre, int publicationYear) 
        : base(title, MediaType.Novel, publicationYear)
    {
        Author = author;
        Genre = genre;
    }

    public override string GetDetails()
    {
        return $"小说: {Title}\n作者: {Author}\n类型: {Genre}\n出版年份: {PublicationYear}\n状态: {Status}";
    }
}

// 杂志类
public class Magazine : LibraryItem
{
    public int IssueNumber { get; }
    public string Publisher { get; }

    public Magazine(string title, string publisher, int issueNumber, int publicationYear) 
        : base(title, MediaType.Magazine, publicationYear)
    {
        Publisher = publisher;
        IssueNumber = issueNumber;
    }

    public override string GetDetails()
    {
        return $"杂志: {Title}\n出版社: {Publisher}\n期号: {IssueNumber}\n出版年份: {PublicationYear}\n状态: {Status}";
    }
}

// 教科书类
public class Textbook : LibraryItem
{
    public string Author { get; }
    public string Subject { get; } // 学科
    public string Publisher { get; }

    public Textbook(string title, string author, string subject, string publisher, int publicationYear) 
        : base(title, MediaType.Textbook, publicationYear)
    {
        Author = author;
        Subject = subject;
        Publisher = publisher;
    }

    public override string GetDetails()
    {
        return $"教科书: {Title}\n作者: {Author}\n学科: {Subject}\n出版社: {Publisher}\n出版年份: {PublicationYear}\n状态: {Status}";
    }
}

// 会员类
public class Member
{
    public string Name { get; }
    public int MemberId { get; }
    public DateTime JoinDate { get; }
    private List<LibraryItem> _borrowedItems;
    
    // 内部计数器
    private static int _nextMemberId = 1001;

    public Member(string name)
    {
        Name = name;
        MemberId = _nextMemberId++;
        JoinDate = DateTime.Now;
        _borrowedItems = new List<LibraryItem>();
    }

    // 借阅物品
    public string BorrowItem(LibraryItem item)
    {
        if (_borrowedItems.Count >= 3)
        {
            return $"{Name}，您已借阅{_borrowedItems.Count}件物品，不能借阅更多。";
        }

        try
        {
            item.MarkAsBorrowed();
            _borrowedItems.Add(item);
            return $"{Name} 成功借阅: {item.Title}";
        }
        catch (InvalidOperationException ex)
        {
            return $"借阅失败: {ex.Message}";
        }
    }

    // 归还物品
    public string ReturnItem(LibraryItem item)
    {
        if (_borrowedItems.Contains(item))
        {
            item.MarkAsAvailable();
            _borrowedItems.Remove(item);
            return $"{Name} 成功归还: {item.Title}";
        }

        return $"{Name} 并未借阅此物品: {item.Title}";
    }

    // 获取借阅列表
    public List<LibraryItem> GetBorrowedItems()
    {
        return new List<LibraryItem>(_borrowedItems);
    }
    
    // 显示借阅信息
    public void DisplayBorrowedItems()
    {
        if (_borrowedItems.Count == 0)
        {
            Console.WriteLine($"{Name} 目前没有借阅任何物品");
            return;
        }
        
        Console.WriteLine($"{Name} 的借阅列表:");
        foreach (var item in _borrowedItems)
        {
            Console.WriteLine($"  - {item.Title} ({item.MediaType})");
        }
    }
    
    // 重写ToString方法
    public override string ToString()
    {
        return $"{MemberId}: {Name} (加入日期: {JoinDate:yyyy-MM-dd})";
    }
}

// 图书馆管理类
public class LibraryManager
{
    private List<LibraryItem> _catalog;
    private List<Member> _members;

    public LibraryManager()
    {
        _catalog = new List<LibraryItem>();
        _members = new List<Member>();
    }

    // 添加物品到目录
    public void AddItem(LibraryItem item)
    {
        _catalog.Add(item);
        Console.WriteLine($"已添加: {item.Title}");
    }

    // 注册会员
    public void RegisterMember(Member member)
    {
        _members.Add(member);
        Console.WriteLine($"已注册会员: {member.Name}");
    }

    // 显示目录
    public void DisplayCatalog()
    {
        Console.WriteLine("\n=== 图书馆目录 ===");
        
        if (_catalog.Count == 0)
        {
            Console.WriteLine("目录为空");
            return;
        }
        
        // 按类型分组显示
        var groupedItems = _catalog.GroupBy(item => item.MediaType);
        
        foreach (var group in groupedItems)
        {
            Console.WriteLine($"\n{group.Key}:");
            foreach (var item in group)
            {
                Console.WriteLine($"  {item}");
            }
        }
    }

    // 按ID查找物品
    public LibraryItem FindItemById(int id)
    {
        return _catalog.FirstOrDefault(item => item.Id == id);
    }

    // 按名称查找会员
    public Member FindMemberByName(string name)
    {
        return _members.FirstOrDefault(member => 
            member.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
    
    // 显示所有会员
    public void DisplayMembers()
    {
        Console.WriteLine("\n=== 注册会员 ===");
        
        if (_members.Count == 0)
        {
            Console.WriteLine("暂无注册会员");
            return;
        }
        
        foreach (var member in _members)
        {
            Console.WriteLine(member);
            member.DisplayBorrowedItems();
            Console.WriteLine();
        }
    }
    
    // 获取目录统计信息
    public void DisplayStats()
    {
        Console.WriteLine("\n=== 图书馆统计 ===");
        Console.WriteLine($"物品总数: {_catalog.Count}");
        Console.WriteLine($"注册会员数: {_members.Count}");
        
        var borrowedCount = _catalog.Count(item => item.Status == BorrowStatus.Borrowed);
        Console.WriteLine($"已借出物品: {borrowedCount}");
        
        // 按类型统计
        var typeStats = _catalog.GroupBy(item => item.MediaType)
                               .Select(g => new { Type = g.Key, Count = g.Count() });
        
        Console.WriteLine("\n按类型统计:");
        foreach (var stat in typeStats)
        {
            Console.WriteLine($"  {stat.Type}: {stat.Count}件");
        }
    }
}

// 主程序

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("欢迎使用图书馆管理系统!");
        Console.WriteLine("=====================\n");

        // 创建图书馆管理器
        LibraryManager library = new LibraryManager();

        // 添加一些图书资料
        library.AddItem(new Novel("百年孤独", "加西亚·马尔克斯", "魔幻现实主义", 1967));
        library.AddItem(new Novel("围城", "钱钟书", "讽刺小说", 1947));
        library.AddItem(new Magazine("国家地理", "国家地理学会", 245, 2023));
        library.AddItem(new Textbook("C#高级编程", "Christian Nagel", "计算机科学", "清华大学出版社", 2022));
        library.AddItem(new Textbook("数据结构与算法", "严蔚敏", "计算机科学", "清华大学出版社", 2020));

        // 注册会员
        Member alice = new Member("张三");
        Member bob = new Member("李四");
        library.RegisterMember(alice);
        library.RegisterMember(bob);

        // 显示目录
        library.DisplayCatalog();

        // 显示统计信息
        library.DisplayStats();

        Console.WriteLine("\n=== 借阅操作演示 ===");

        // 张三借阅几本书
        LibraryItem item1 = library.FindItemById(1);
        if (item1 != null)
            Console.WriteLine(alice.BorrowItem(item1));

        LibraryItem item2 = library.FindItemById(3);
        if (item2 != null)
            Console.WriteLine(alice.BorrowItem(item2));

        LibraryItem item3 = library.FindItemById(4);
        if (item3 != null)
            Console.WriteLine(alice.BorrowItem(item3));

        // 尝试借阅第四本（应该失败）
        LibraryItem item4 = library.FindItemById(2);
        if (item4 != null)
            Console.WriteLine(alice.BorrowItem(item4));

        // 李四借阅一本书
        if (item4 != null)
            Console.WriteLine(bob.BorrowItem(item4));

        // 显示会员信息
        library.DisplayMembers();

        // 归还一本书
        if (item1 != null)
            Console.WriteLine(alice.ReturnItem(item1));

        // 再次显示会员信息
        library.DisplayMembers();

        // 显示更新后的统计信息
        library.DisplayStats();

        // 显示某本书的详细信息
        if (item4 != null)
        {
            Console.WriteLine("\n=== 物品详细信息 ===");
            Console.WriteLine(item4.GetDetails());
        }

        Console.WriteLine("\n感谢使用图书馆管理系统!");
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }
}