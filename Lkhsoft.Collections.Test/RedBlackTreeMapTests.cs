using System.Text.Json;
using System.Xml.Serialization;
using Lkhsoft.Collections.Bst;
using NUnit.Framework;

namespace Lkhsoft.Collections.Test;

 [TestFixture]
    public class RedBlackTreeMapTests
    {
        [Test]
        public void AddAndGet_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"three", 3}
            };

            Assert.That(tree["one"], Is.EqualTo(1));
            Assert.That(tree["two"], Is.EqualTo(2));
            Assert.That(tree["three"], Is.EqualTo(3));
        }

        [Test]
        public void Remove_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2},
                {"three", 3}
            };

            Assert.That(tree.Remove("two"), Is.True);
            Assert.That(tree.ContainsKey("two"), Is.False);
            Assert.That(tree.Count, Is.EqualTo(2));
        }

        [Test]
        public void ContainsKey_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2}
            };

            Assert.That(tree.ContainsKey("one"), Is.True);
            Assert.That(tree.ContainsKey("two"), Is.True);
            Assert.That(tree.ContainsKey("three"), Is.False);
        }

        [Test]
        public void TryGetValue_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2}
            };

            Assert.That(tree.TryGetValue("one", out var value), Is.True);
            Assert.That(value, Is.EqualTo(1));

            Assert.That(tree.TryGetValue("three", out value), Is.False);
            Assert.That(value, Is.EqualTo(default(int)));
        }

        [Test]
        public void Clear_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2}
            };

            tree.Clear();
            Assert.That(tree.Count, Is.EqualTo(0));
            Assert.That(tree.ContainsKey("one"), Is.False);
            Assert.That(tree.ContainsKey("two"), Is.False);
        }

        [Test]
        public void JsonSerialization_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2}
            };

            var json = JsonSerializer.Serialize(tree);
            var deserializedTree = JsonSerializer.Deserialize<RedBlackTree<string, int>>(json);

            Assert.That(deserializedTree.Count, Is.EqualTo(2));
            Assert.That(deserializedTree.ContainsKey("one"), Is.True);
            Assert.That(deserializedTree.ContainsKey("two"), Is.True);
            Assert.That(deserializedTree["one"], Is.EqualTo(1));
            Assert.That(deserializedTree["two"], Is.EqualTo(2));
        }

        [Test]
        public void XmlSerialization_ShouldWork()
        {
            var tree = new RedBlackTree<string, int>
            {
                {"one", 1},
                {"two", 2}
            };

            var serializer = new XmlSerializer(typeof(RedBlackTree<string, int>));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, tree);
            var xml = stringWriter.ToString();

            using var stringReader = new StringReader(xml);
            var deserializedTree = (RedBlackTree<string, int>)serializer.Deserialize(stringReader)!;

            Assert.That(deserializedTree.Count, Is.EqualTo(2));
            Assert.That(deserializedTree.ContainsKey("one"), Is.True);
            Assert.That(deserializedTree.ContainsKey("two"), Is.True);
            Assert.That(deserializedTree["one"], Is.EqualTo(1));
            Assert.That(deserializedTree["two"], Is.EqualTo(2));
        }
    }