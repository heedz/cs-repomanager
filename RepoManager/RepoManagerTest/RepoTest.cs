using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoManager;

namespace RepoManagerTest
{
    [TestClass]
    public class RepoTest
    {
        [TestMethod]
        public void RegisterMethod()
        {
            RepoManager.RepoManager RM = new RepoManager.RepoManager();

            //Register unregistered content XML
            Assert.AreEqual(
                ReturnState.Success,
                RM.Register<string>("coba1", "content1", new RepoManager.XmlItemType())
                );
            //Register registered content XML
            Assert.AreEqual(
                ReturnState.ItemExist,
                RM.Register<string>("coba1", "content1", new RepoManager.XmlItemType())
                );
            //Register unregistered content XML 2 
            Assert.AreEqual(
                ReturnState.Success,
                RM.Register<string>("coba2", "content2", new RepoManager.XmlItemType())
                );
        }

        [TestMethod]
        public void DeregisterMethod()
        {
            RepoManager.RepoManager RM = new RepoManager.RepoManager();

            //Deregister unregistered content
            Assert.AreEqual(
                ReturnState.ItemNotFound,
                RM.Deregister("coba1")
                );

            //Deregister registered content
            Assert.AreEqual(
                ReturnState.Success,
                RM.Register<string>("coba1", "content1", new RepoManager.XmlItemType())
                );

            Assert.AreEqual(
                ReturnState.Success,
                RM.Deregister("coba1")
                );
        }

        [TestMethod]
        public void GetContentMethod()
        {
            RepoManager.RepoManager RM = new RepoManager.RepoManager();
            Tuple<string, ReturnState> returnValue;

            //GetContent from unregistered content
            returnValue = RM.GetContent<string>("coba1");

            Assert.AreEqual(
                null,
                returnValue.Item1
                );
            Assert.AreEqual(
                ReturnState.ItemNotFound,
                returnValue.Item2
                );

            //GetContent from registered content
            Assert.AreEqual(
                ReturnState.Success,
                RM.Register<string>("coba1", "content1", new RepoManager.XmlItemType())
                );

            returnValue = RM.GetContent<string>("coba1");

            Assert.AreEqual(
                "content1",
                returnValue.Item1
                );
            Assert.AreEqual(
                ReturnState.Success,
                returnValue.Item2
                );

            //GetContent with wrong type
            returnValue = RM.GetContent<int>("coba1");

            Assert.AreEqual(
                null,
                returnValue.Item1
                );
            Assert.AreEqual(
                ReturnState.ItemTypeIsNotValid,
                returnValue.Item2
                );
        }
    }
}
