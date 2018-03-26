using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepoManager
{
    public interface IItemType      /* Interface untuk membuat kontrak bahwa ItemType harus mempunyai ItemTypeNum dan */
    {                               /* method Validate */
        int ItemTypeNum { get; }

        bool Validate<T>(T content);
    }

    public class XmlItemType : IItemType    /* Implementasi item type XML */
    {
        public int ItemTypeNum { get; }

        public XmlItemType()
        {
            ItemTypeNum = 0;
        }

        public bool Validate<T>(T content)
        {
            return true;
        }
    }

    public class JsonItemType : IItemType   /* Implementasi item type JSON */
    {
        public int ItemTypeNum { get; }

        public JsonItemType()
        {
            ItemTypeNum = 1;
        }

        public bool Validate<T>(T content)
        {
            return true;
        }
    }

    public interface IItem      /* Interface untuk membuat kontrak bahwa Item harus mempunyai content dan itemType */
    {
        IItemType ContentItemType { get; }
    }

    public class Item<T> : IItem   /* Implementasi dari interface Item */
    {
        public T Content { get; private set; }
        public IItemType ContentItemType { get; private set; }

        public Item(T content, IItemType itemType)
        {
            Content = content;
            ContentItemType = itemType;
        }
    }

    public enum ReturnState
    {
        ItemNotFound = -1,
        ContentIsNull = -2,
        ItemExist = -3,
        ContentIsNotValid = -4,
        ItemTypeIsNotValid = -5,
        NameIsNull = -6,
        Success = 0
    }

    public class RepoData
    {
        static RepoData _instance;
        static object _locker = new object();

        Dictionary<string, IItem> _items;

        private RepoData()
        {
            _items = new Dictionary<string, IItem>();
        }

        public static RepoData GetInstance()
        {
            lock (_locker)
            {
                if (_instance == null)
                    _instance = new RepoData();
            }

            return _instance;
        }

        public Dictionary<string, IItem> GetItems()
        {
            return _items;
        }
    }

    public class RepoManager
    {
        static object _locker = new object();
        private RepoData _repoData = RepoData.GetInstance();

        public RepoManager() { }

        public ReturnState Register<T>(string name, T content, IItemType itemType)
        {
            if (content == null)
                return ReturnState.ContentIsNull;

            if (!itemType.Validate(content))
                return ReturnState.ContentIsNotValid;

            lock (_locker)
            {
                if (Contains(name))
                    return ReturnState.ItemExist;
           
                _repoData.GetItems().Add(name, new Item<T>(content, itemType));
            }
            
            return ReturnState.Success;
        }

        private bool Contains(string name)
        {
            lock (_locker)
            {
                return _repoData.GetItems().ContainsKey(name);
            }
        }

        public ReturnState Deregister(string name)
        {
            if (name == null)
                return ReturnState.NameIsNull;

            lock (_locker)
            {
                if (!Contains(name))
                    return ReturnState.ItemNotFound;

                _repoData.GetItems().Remove(name);

            }

            return ReturnState.Success;
        }

        public Tuple<string, ReturnState> GetContent<T>(string name)    /* Mengembalikan object karena content dapat berupa apapun */
        {
            Item<T> temp = null;

            lock (_locker)
            {
                if (!Contains(name))
                    return new Tuple<string, ReturnState>(null, ReturnState.ItemNotFound);
            
                temp = _repoData.GetItems()[name] as Item<T>;
            }

            if (temp == null)
                return new Tuple<string, ReturnState>(null, ReturnState.ItemTypeIsNotValid);

            return new Tuple<string, ReturnState>(temp.Content.ToString(), ReturnState.Success);
        }
    }
}
