using System;
using RustMacroExpander;
using Microsoft.CSharp;
using Xunit;
using RustMacroExpander.ViewModels;

namespace RustMacroExpander.Tests
{
    public class UnitTest1
    {
        public class Person
        {
            public string Trait { get; set; } = "Ugly";
            public uint Age { get; set; } = 10;
        }

        public class ViewModel : ViewModelBase
        {
            public ViewModel() : base(new Person())
            {
            }
        }

        [Fact]
        public void PropertyAvailability()
        {
            dynamic vm = new ViewModel();
            dynamic trait = vm.Trait;

            Assert.Equal(trait, "Ugly");
            Assert.Equal<uint>(vm.Age, 10);
        }
    }
}
