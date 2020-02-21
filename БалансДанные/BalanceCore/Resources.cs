using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Resources;

namespace ClassLibrary1.Properties
{
    internal class Resources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Resources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)ClassLibrary1.Properties.Resources.resourceMan, (object)null))
                    ClassLibrary1.Properties.Resources.resourceMan = new ResourceManager("ClassLibrary1.Properties.Resources", typeof(ClassLibrary1.Properties.Resources).Assembly);
                return ClassLibrary1.Properties.Resources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => ClassLibrary1.Properties.Resources.resourceCulture;
            set => ClassLibrary1.Properties.Resources.resourceCulture = value;
        }

        internal static Icon add => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(add), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon AddToBase => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(AddToBase), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon Base => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(Base), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon copy => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(copy), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static string defaultDB => ClassLibrary1.Properties.Resources.ResourceManager.GetString(nameof(defaultDB), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon del => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(del), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon delall => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(delall), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon DelToBase => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(DelToBase), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon excel => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(excel), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon insert => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(insert), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon last => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(last), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon newfile => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(newfile), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon next => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(next), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon open => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(open), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static string regPathMain => ClassLibrary1.Properties.Resources.ResourceManager.GetString(nameof(regPathMain), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon save => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(save), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static Icon save_as => (Icon)ClassLibrary1.Properties.Resources.ResourceManager.GetObject(nameof(save_as), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static string tableFolder => ClassLibrary1.Properties.Resources.ResourceManager.GetString(nameof(tableFolder), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static string tableReg => ClassLibrary1.Properties.Resources.ResourceManager.GetString(nameof(tableReg), ClassLibrary1.Properties.Resources.resourceCulture);

        internal static string userFolder => ClassLibrary1.Properties.Resources.ResourceManager.GetString(nameof(userFolder), ClassLibrary1.Properties.Resources.resourceCulture);
    }
}