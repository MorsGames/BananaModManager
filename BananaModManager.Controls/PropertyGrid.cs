using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace BananaModManager.Controls
{
    public partial class PropertyGrid : System.Windows.Forms.PropertyGrid
    {
        public PropertyGrid(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Resize += MoveSplitter;
            SelectedObjectsChanged += MoveSplitter;
            PropertySortChanged += ResetSorting;

            // Hide the unused button and splitter
            var buttons = Controls.OfType<ToolStrip>().FirstOrDefault()?.Items;
            buttons[buttons.Count - 1].Visible = false;
            buttons[buttons.Count - 2].Visible = false;

            // Resize the help section
            var docComment = Controls[0];
            var fieldInfo = docComment.GetType().BaseType
                .GetField("userSized", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(docComment, true);
            docComment.Height = 80;
        }

        public void MoveSplitter(object _, EventArgs __)
        {
            // Change the splitter location
            var propertyGridView = Controls[2];
            var methodInfo = propertyGridView.GetType()
                .GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(propertyGridView, new object[] {(int) (Width * 0.625)});
        }

        private void ResetSorting(object _, EventArgs __)
        {
            if (PropertySort == PropertySort.CategorizedAlphabetical) PropertySort = PropertySort.Categorized;
        }

        public void ChangeRenderer()
        {
            ToolStripRenderer = ToolStripManager.Renderer;
        }
    }
}