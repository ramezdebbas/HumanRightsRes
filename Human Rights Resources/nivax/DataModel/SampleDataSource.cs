using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace HealthAndFitness.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : HealthAndFitness.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");
      
        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }


        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Nivax Sample Data Source");

            var group1 = new SampleDataGroup("Group-1",
                 "Introduction",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group1.Items.Add(new SampleDataItem("BigL-Group-1-Item1",
                 "The Way",
                 "",
                 "Assets/HubPageImages/HubPageImage1.png",
                 "Human rights are rights inherent to all human beings, whatever our nationality, place of residence, sex, national or ethnic origin, colour, religion, language, or any other status. We are all equally entitled to our human rights without discrimination. These rights are all interrelated, interdependent and indivisible.",
                 "\n\nHuman rights are commonly understood as inalienable fundamental rights to which a person is inherently entitled simply because she or he is a human being. Human rights are thus conceived as universal (applicable everywhere) and egalitarian (the same for everyone). These rights may exist as natural rights or as legal rights, in local, regional, national, and international law. The doctrine of human rights in international practice, within international law, global and regional institutions, in the policies of states and in the activities of non-governmental organizations, has been a cornerstone of public policy around the world. The idea of human rights states, if the public discourse of peacetime global society can be said to have a common moral language, it is that of human rights. Despite this, the strong claims made by the doctrine of human rights continue to provoke considerable skepticism and debates about the content, nature and justifications of human rights to this day. Indeed, the question of what is meant by a right is itself controversial and the subject of continued philosophical debate.\n\nMany of the basic ideas that animated the human rights movement developed in the aftermath of the Second World War and the atrocities of The Holocaust, culminating in the adoption of the Universal Declaration of Human Rights in Paris by the United Nations General Assembly in 1948. The ancient world did not possess the concept of universal human rights\n\n The true forerunner of human rights discourse was the concept of natural rights which appeared as part of the medieval Natural law tradition that became prominent during the Enlightenment with such philosophers as John Locke, Francis Hutcheson, and Jean-Jacques Burlamaqui, and featured prominently in the political discourse of the American Revolution and the French Revolution.",
                 67,
                 104,
                 group1));
            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "What are human rights?",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("ServiceL-Group-2-Item1",
                "Step-in",
                "",
                "Assets/HubPageImages/HubPageImage2.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "\n\n\n\n\n\n\n\n\n\nHuman rights are rights inherent to all human beings, whatever our nationality, place of residence, sex, national or ethnic origin, colour, religion, language, or any other status. We are all equally entitled to our human rights without discrimination. These rights are all interrelated, interdependent and indivisible.\n\nUniversal human rights are often expressed and guaranteed by law, in the forms of treaties, customary international law , general principles and other sources of international law. International human rights law lays down obligations of Governments to act in certain ways or to refrain from certain acts, in order to promote and protect human rights and fundamental freedoms of individuals or groups.",
                43,
                26,
                group2));
            group2.Items.Add(new SampleDataItem("ServiceL-Group-2-Item2",
               "Universal and Inalienable",
               "",
               "Assets/HubPageImages/HubPageImage3.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "\n\n\n\n\n\n\n\n\n\nThe principle of universality of human rights is the cornerstone of international human rights law. This principle, as first emphasized in the Universal Declaration on Human Rights in 1948, has been reiterated in numerous international human rights conventions, declarations, and resolutions. The 1993 Vienna World Conference on Human Rights, for example, noted that it is the duty of States to promote and protect all human rights and fundamental freedoms, regardless of their political, economic and cultural systems.\n\nAll States have ratified at least one, and 80% of States have ratified four or more, of the core human rights treaties, reflecting consent of States which creates legal obligations for them and giving concrete expression to universality. Some fundamental human rights norms enjoy universal protection by customary international law across all boundaries and civilizations.\n\nHuman rights are inalienable. They should not be taken away, except in specific situations and according to due process. For example, the right to liberty may be restricted if a person is found guilty of a crime by a court of law.",
               43,
               26,
               group2));
            group2.Items.Add(new SampleDataItem("ServiceP-Group-2-Item3",
               "Interdependent and indivisible",
               "",
               "Assets/HubPageImages/HubPageImage4.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "\n\n\n\n\n\n\n\n\n\nAll human rights are indivisible, whether they are civil and political rights, such as the right to life, equality before the law and freedom of expression; economic, social and cultural rights, such as the rights to work, social security and education , or collective rights, such as the rights to development and self-determination, are indivisible, interrelated and interdependent. The improvement of one right facilitates advancement of the others. Likewise, the deprivation of one right adversely affects the others.",
               26,
               52,
               group2));
            group2.Items.Add(new SampleDataItem("ServiceL-Group-2-Item4",
               "Equal and non-discriminatory",
               "",
               "Assets/HubPageImages/HubPageImage5.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "\n\n\n\n\n\n\n\n\n\nNon-discrimination is a cross-cutting principle in international human rights law. The principle is present in all the major human rights treaties and provides the central theme of some of international human rights conventions such as the International Convention on the Elimination of All Forms of Racial Discrimination and the Convention on the Elimination of All Forms of Discrimination against Women.  \n\nThe principle applies to everyone in relation to all human rights and freedoms and it prohibits discrimination on the basis of a list of non-exhaustive categories such as sex, race, colour and so on. The principle of non-discrimination is complemented by the principle of equality, as stated in Article 1 of the Universal Declaration of Human Rights: All human beings are born free and equal in dignity and rights",
               43,
               26,
               group2));
            group2.Items.Add(new SampleDataItem("ServiceL-Group-2-Item5",
               "Both Rights and Obligations",
               "",
               "Assets/HubPageImages/HubPageImage6.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "\n\n\n\n\n\n\n\n\n\nHuman rights entail both rights and obligations. States assume obligations and duties under international law to respect, to protect and to fulfil human rights. The obligation to respect means that States must refrain from interfering with or curtailing the enjoyment of human rights. The obligation to protect requires States to protect individuals and groups against human rights abuses. The obligation to fulfil means that States must take positive action to facilitate the enjoyment of basic human rights. At the individual level, while we are entitled our human rights, we should also respect the human rights of others.",
               43,
               26,
               group2));
            group2.Items.Add(new SampleDataItem("ServiceP-Group-2-Item6",
               "Criticism",
               "",
               "Assets/HubPageImages/HubPageImage7.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "\n\n\n\n\n\n\n\n\n\nThe claims made by human rights to universality have led to criticism. Philosophers who have criticized the concept of human rights include Jeremy Bentham, Edmund Burke, Friedrich Nietzsche and Karl Marx. Political philosophy professor Charles Blattberg argues that discussion of human rights, being abstract, demotivates people from upholding the values that rights are meant to affirm. The Internet Encyclopedia of Philosophy gives particular attention to two types of criticisms: the one questioning universality of human rights and the one denying them objective ground.Alain Pellet, an international law scholar, criticizes human rightism approach as denying the principle of sovereignty and claiming a special place for human rights among the branches of international law Alain de Benoist questions human rights premises of human equality. David Kennedy had listed pragmatic worries and polemical charges concerning human rights in 2002 in Harvard Human Rights Journal.",
               26,
               52,
               group2));

            this.AllGroups.Add(group2);

            var group3 = new SampleDataGroup("Group-3",
                "Classifications",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("GearSmall-Group-3-Item1",
                "Indivisibility",
                "",
                "Assets/HubPageImages/HubPageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "\n\n\n\n\n\n\n\n\n\nThe UDHR included both economic, social and cultural rights and civil and political rights because it was based on the principle that the different rights could only successfully exist in combination:\n\nThe ideal of free human beings enjoying civil and political freedom and freedom from fear and want can only be achieved if conditions are created whereby everyone may enjoy his civil and political rights, as well as his social, economic and cultural rights.\n\nInternational Covenant on Civil and Political Rights and the International Covenant on Economic Social and Cultural Rights, 1966. This is held to be true because without civil and political rights the public cannot assert their economic, social and cultural rights. Similarly, without livelihoods and a working society, the public cannot assert or make use of civil or political rights (known as the full belly thesis). The indivisibility and interdependence of all human rights has been confirmed by the 1993 Vienna Declaration and Programme of Action \n\nAll human rights are universal, indivisible and interdependent and related. The international community must treat human rights globally in a fair and equal manner, on the same footing, and with the same emphasis. Vienna Declaration and Programme of Action, World Conference on Human Rights, 1993 This statement was again endorsed at the 2005 World Summit in New York. \n\nAlthough accepted by the signatories to the UDHR, most do not in practice give equal weight to the different types of rights. Some Western cultures have often given priority to civil and political rights, sometimes at the expense of economic and social rights such as the right to work, to education, health and housing. Similarly the ex Soviet bloc countries and Asian countries have tended to give priority to economic, social and cultural rights, but have often failed to provide civil and political rights.",
                42,
                52,
                group3));
			group3.Items.Add(new SampleDataItem("GearBig-Group-3-Item2",
                "Categorization",
                "",
                "Assets/HubPageImages/HubPageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "\n\n\n\n\n\n\n\n\n\nOpponents of the indivisibility of human rights argue that economic, social and cultural rights are fundamentally different from civil and political rights and require completely different approaches. Economic, social and cultural rights are argued to be positive, meaning that they require active provision of entitlements by the state (as opposed to the state being required only to prevent the breach of rights) resource-intensive, meaning that they are expensive and difficult to provide\nprogressive, meaning that they will take significant time to implement\nvague, meaning they cannot be quantitatively measured, and whether they are adequately provided or not is difficult to judge\nideologically divisive/political, meaning that there is no consensus on what should and shouldn't be provided as a right\nsocialist, as opposed to capitalist\nnon-justiciable, meaning that their provision, or the breach of them, cannot be judged in a court of law\naspirations or goals, as opposed to real 'legal' rights\nSimilarly civil and political rights are categorized as:\nnegative, meaning the state can protect them simply by taking no action\ncost-free\nimmediate, meaning they can be immediately provided if the state decides to\nprecise, meaning their provision is easy to judge and measure\nnon-ideological/non-political\ncapitalist\njusticiable\nreal legal rights\n\nOlivia Ball and Paul Gready argue that for both civil and political rights and economic, social and cultural rights, it is easy to find examples which do not fit into the above categorisation. Among several others, they highlight the fact that maintaining a judicial system, a fundamental requirement of the civil right to due process before the law and other rights relating to judicial process, is positive, resource-intensive, progressive and vague, while the social right to housing is precise, justiciable and can be a real 'legal' right.",
                84,
                52,
                group3));
			group3.Items.Add(new SampleDataItem("GearSmall-Group-3-Item3",
                "Three generations",
                "",
                "Assets/HubPageImages/HubPageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "\n\n\n\n\n\n\n\n\n\nAnother categorization, offered by Karel Vasak, is that there are three generations of human rights: first-generation civil and political rights (right to life and political participation), second-generation economic, social and cultural rights (right to subsistence) and third-generation solidarity rights (right to peace, right to clean environment). Out of these generations, the third generation is the most debated and lacks both legal and political recognition. This categorisation is at odds with the indivisibility of rights, as it implicitly states that some rights can exist without others. Prioritisation of rights for pragmatic reasons is however a widely accepted necessity. Human rights expert Philip Alston argues",
                42,
                52,
                group3));
            this.AllGroups.Add(group3);


            var group4 = new SampleDataGroup("Group-4",
                "International Protection",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group4.Items.Add(new SampleDataItem("LatestNews-Group-4-Item1",
                "United Nations Charter",
                "",
                "",
                "The provisions of the United Nations Charter provided a basis for the development of international human rights protection. The preamble of the charter provides that the members reaffirm faith in fundamental human rights, in the equal rights of men and women",
                "Of particular importance is Article 56 of the charter: All Members pledge themselves to take joint and separate action in co-operation with the Organization for the achievement of the purposes set forth in Article 55. This is a binding treaty provision applicable to both the Organisation and its members and has been taken to constitute a legal obligation for the members of the United Nations. Overall, the references to human rights in the Charter are general and vague. The Charter does not contain specific legal rights, nor does it mandate any enforcement procedures to protect these rights. Despite this, the significance of the espousal of human rights within the UN charter must not be understated. The importance of human rights on the global stage can be traced to the importance of human rights within the United Nations framework and the UN Charter can be seen as the starting point for the development of a broad array of declarations, treaties, implementation and enforcement mechanisms, UN organs, committees and reports on the protection of human rights. The rights espoused in the UN charter would be codified and defined in the International Bill of Human Rights, composing the Universal Declaration of Human Rights, the International Covenant on Civil and Political Rights and the International Covenant on Economic, Social and Cultural Rights.",
                79,
                21,
                group4));
			
			group4.Items.Add(new SampleDataItem("LatestNews-Group-4-Item2",
                "Universal Declaration of Human Rights",
                "",
                "",
                "The Universal Declaration of Human Rights (UDHR) was adopted by the United Nations General Assembly in 1948, partly in response to the atrocities of World War II.",
                "The UDHR was framed by members of the Human Rights Commission, with former First Lady Eleanor Roosevelt as Chair, who began to discuss an International Bill of Rights in 1947. The members of the Commission did not immediately agree on the form of such a bill of rights, and whether, or how, it should be enforced. The Commission proceeded to frame the UDHR and accompanying treaties, but the UDHR quickly became the priority. Canadian law professor John Humphrey and French lawyer René Cassin were responsible for much of the cross-national research and the structure of the document respectively, where the articles of the declaration were interpretative of the general principle of the preamble. The document was structured by Cassin to include the basic principles of dignity, liberty, equality and brotherhood in the first two articles, followed successively by rights pertaining to individuals; rights of individuals in relation to each other and to groups; spiritual, public and political rights; and economic, social and cultural rights. The final three articles place, according to Cassin, rights in the context of limits, duties and the social and political order in which they are to be realized. Humphrey and Cassin intended the rights in the UDHR to be legally enforceable through some means, as is reflected in the third clause of the preamble",
                79,
                21,
                group4));
			
			group4.Items.Add(new SampleDataItem("LatestNews-Group-4-Item3",
                "Customary international law",
                "",
                "",
                "Customary international law are those aspects of international law that derive from custom. Along with general principles of law and treaties, custom is considered by the International Court of Justice, jurists, the United Nations, and its member states to be among the primary sources of international law.",
                "The International Court of Justice Statute defines customary international law in Article 38(1)(b) as evidence of a general practice accepted as law. This is generally determined through two factors: the general practice of states and what states have accepted as law. There are several different kinds of customary international laws recognized by states. Some customary international laws rise to the level of jus cogens through acceptance by the international community as non-derogate able rights, while other customary international law may simply be followed by a small group of states. States are typically bound by customary international law regardless of whether the states have codified these laws domestically or through treaties.",
                79,
                21,
                group4));
			
			group4.Items.Add(new SampleDataItem("LatestNews-Group-4-Item4",
                "International humanitarian law",
                "",
                "",
                "International humanitarian law (IHL), or the law of armed conflict, is the law that regulates the conduct of armed conflicts (jus in bello). It comprises the Geneva Conventions and the Hague Conventions, as well as subsequent treaties, case law, and customary international law.",
                "Modern International Humanitarian Law is made up of two historical streams: the law of The Hague referred to in the past as the law of war proper and the law of Geneva or humanitarian law.[3] The two streams take their names from a number of international conferences which drew up treaties relating to war and conflict, in particular the Hague Conventions of 1899 and 1907, and the Geneva Conventions, the first which was drawn up in 1863. Both are branches of jus in bello, international law regarding acceptable practices while engaged in war and armed conflict.\n\nThe Law of The Hague, or the Laws of War proper,determines the rights and duties of belligerents in the conduct of operations and limits the choice of means in doing harm. In particular, it concerns itself with the definition of combatants, establishes rules relating to the means and methods of warfare, and examines the issue of military objectives.",
                79,
                21,
                group4));
			
			group4.Items.Add(new SampleDataItem("LatestNews-Group-4-Item5",
                "United Nations",
                "",
                "",
                "The United Nations (UN; French: Organisation des Nations Unies, ONU) is the world's largest, foremost, and most prominent international organization.",
                "The United Nations (UN; French: Organisation des Nations Unies, ONU) is the world's largest, foremost, and most prominent international organization. The stated aims of the United Nations include promoting and facilitating cooperation in international law, international security, economic development, social progress, human rights, civil rights, civil liberties, political freedoms, democracy, and the achievement of lasting world peace. The UN was founded in 1945 after World War II to replace the League of Nations, to stop wars between countries, and to provide a platform for dialogue. It contains multiple subsidiary organizations to carry out its missions.\n\nAt its founding, the UN had 51 member states; as of 2011, there are 193. From its offices around the world, the UN and its specialized agencies decide on substantive and administrative issues in regular meetings held throughout the year. The organization has six principal organs: the General Assembly (the main deliberative assembly); the Security Council (for deciding certain resolutions for peace and security); the Economic and Social Council (for assisting in promoting international economic and social cooperation and development); the Secretariat (for providing studies, information, and facilities needed by the UN); the International Court of Justice (the primary judicial organ); and the United Nations Trusteeship Council (which is currently inactive). Other prominent UN System agencies include the World Health Organization (WHO), the World Food Programme (WFP) and United Nations Children's Fund (UNICEF). The UN's most prominent position is that of the office of Secretary-General which has been held by Ban Ki-moon of South Korea since 2007.",
                79,
                21,
                group4));
            this.AllGroups.Add(group4);
			
			
			var group5 = new SampleDataGroup("Group-5",
                "Substantive Rights",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group5.Items.Add(new SampleDataItem("RecipesBig-Group-5-Item1",
                "Freedom from torture",
                "",
                "Assets/HubPageImages/HubPageImage11.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nThroughout history, torture has been used as a method of political re-education, interrogation, punishment, and coercion. In addition to state-sponsored torture, individuals or groups may be motivated to inflict torture on others for similar reasons to those of a state; however, the motive for torture can also be for the sadistic gratification of the torturer, as in the Moors murders.\n\nTorture is prohibited under international law and the domestic laws of most countries in the 21st century. It is considered to be a violation of human rights, and is declared to be unacceptable by Article 5 of the UN Universal Declaration of Human Rights. Signatories of the Third Geneva Convention and Fourth Geneva Convention officially agree not to torture prisoners in armed conflicts. Torture is also prohibited by the United Nations Convention Against Torture, which has been ratified by 147 states.\n\nNational and international legal prohibitions on torture derive from a consensus that torture and similar ill-treatment are immoral, as well as impractical. Despite these international conventions, organizations that monitor abuses of human rights (e.g. Amnesty International, the International Rehabilitation Council for Torture Victims) report widespread use condoned by states in many regions of the world. Amnesty International estimates that at least 81 world governments currently practice torture, some of them openly.",
                54,
                104,
                group5));

            group5.Items.Add(new SampleDataItem("RecipesSmall-Group-5-Item2",
                "Freedom from slavery",
                "",
                "Assets/HubPageImages/HubPageImage12.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nFreedom from slavery is an internationally recognized human right. Article 4 of the Universal Declaration of Human Rights states:\n\nNo one shall be held in slavery or servitude; slavery and the slave trade shall be prohibited in all their forms.Despite this, the number of slaves today is higher than at any point in history, remaining as high as 12 million to 27 million, Most are debt slaves, largely in South Asia, who are under debt bondage incurred by lenders, sometimes even for generations. Human trafficking is primarily for prostituting women and children into sex industries.\n\nGroups such as the American Anti-Slavery Group, Anti-Slavery International, Free the Slaves, the Anti-Slavery Society, and the Norwegian Anti-Slavery Society continue to campaign to rid the world of slavery.",
                49,
                35,
                group5));

            group5.Items.Add(new SampleDataItem("RecipesSmall-Group-5-Item3",
                "Right to a fair trial",
                "",
                "Assets/HubPageImages/HubPageImage13.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nThe right to fair trial is an essential right in all countries respecting the rule of law. A trial in these countries that is deemed unfair will typically be restarted, or its verdict voided. Various rights associated with a fair trial are explicitly proclaimed in Article 10 of the Universal Declaration of Human Rights, the Sixth Amendment to the United States Constitution, and Article 6 of the European Convention of Human Rights, as well as numerous other constitutions and declarations throughout the world. There is no binding international law that defines what is or is not a fair trial, for example the right to a jury trial and other important procedures vary from nation to nation.",
                49,
                34,
                group5));

            group5.Items.Add(new SampleDataItem("RecipesSmall-Group-5-Item4",
                "Freedom of speech",
                "",
                "Assets/HubPageImages/HubPageImage14.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nFreedom of speech is the freedom to speak freely without censorship. The term freedom of expression is sometimes used synonymously, but includes any act of seeking, receiving and imparting information or ideas, regardless of the medium used. In practice, the right to freedom of speech is not absolute in any country and the right is commonly subject to limitations, such as on libel, slander, obscenity, incitement to commit a crime, etc. The right to freedom of expression is recognized as a human right under Article 19 of the Universal Declaration of Human Rights and recognized in international human rights law in the International Covenant on Civil and Political Rights (ICCPR). Article 19 of the ICCPR states that veryone shall have the right to hold opinions without interference and everyone shall have the right to freedom of expression; this right shall include freedom to seek, receive and impart information and ideas of all kinds, regardless of frontiers, either orally, in writing or in print, in the form of art, or through any other media of his choice",
                49,
                35,
                group5));

            group5.Items.Add(new SampleDataItem("RecipesSmall-Group-5-Item5",
                "Freedom of thought, conscience and religion",
                "",
                "Assets/HubPageImages/HubPageImage15.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nFreedom of thought, conscience and religion are closely related rights that protect the freedom of an individual or community, in public or private, to think and freely hold conscientious beliefs and to manifest religion or belief in teaching, practice, worship, and observance; the concept is generally recognized also to include the freedom to change religion or not to follow any religion. The freedom to leave or discontinue membership in a religion or religious group—in religious terms called apostasy — is also a fundamental part of religious freedom, covered by Article 18 of the Universal Declaration of Human Rights.\n\nHuman rights groups such as Amnesty International organises campaigns to protect those arrested and or incarcerated as a prisoner of conscience because of their conscientious beliefs, particularly concerning intellectual, political and artistic freedom of expression and association. In legislation, a conscience clause is a provision in a statute that excuses a health professional from complying with the law (for example legalising surgical or pharmaceutical abortion) if it is incompatible with religious or conscientious beliefs.",
                 49,
                35,
                group5));

            group5.Items.Add(new SampleDataItem("RecipesMedium-Group-5-Item6",
                "Freedom of movement",
                "",
                "Assets/HubPageImages/HubPageImage16.png",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sollicitudin, lectus dictum accumsan convallis, tellus sagittis nulla, in tempus magna libero et libero.",
                "\n\n\n\n\n\n\n\n\n\nFreedom of movement, mobility rights or the right to travel is a human right concept that the constitutions of numerous states respect. It asserts that a citizen of a state in which that citizen is present has the liberty to travel, reside in, and/or work in any part of the state where one pleases within the limits of respect for the liberty and rights of others, and to leave that state and return at any time. Some immigrants' rights advocates say that human beings have a fundamental human right to mobility not only within a state but between states.",
                49,
                69,
                group5));
            this.AllGroups.Add(group5);



            var group6 = new SampleDataGroup("Group-6",
                "Relationships",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group6.Items.Add(new SampleDataItem("Interview-Group-6-Item1",
                "Human rights and the environment",
                "",
                "Assets/HubPageImages/HubPageImage17.png",
                "There are two basic conceptions of environmental human rights in the current human rights system. The first is that the right to a healthy or adequate environment is itself a human right (as seen in both Article 24 of the African Charter on Human and Peoples' Rights",
                "\n\n\n\n\n\n\n\n\n\nThere are two basic conceptions of environmental human rights in the current human rights system. The first is that the right to a healthy or adequate environment is itself a human right (as seen in both Article 24 of the African Charter on Human and Peoples' Rights, and Article 11 of the San Salvador Protocol to the American Convention on Human Rights). The second conception is the idea that environmental human rights can be derived from other human rights, usually – the right to life, the right to health, the right to private family life and the right to property (among many others). This second theory enjoys much more widespread use in human rights courts around the world, as those rights are contained in many human rights documents.",
                93,
                35,
                group6));

            group6.Items.Add(new SampleDataItem("Interview-Group-6-Item2",
                "National security",
                "",
                "Assets/HubPageImages/HubPageImage18.png",
                "With the exception of non-derogable human rights international conventions class the right to life, the right to be free from slavery, the right to be free from torture and the right to be free from retroactive application of penal laws as non-derogable",
                "\n\n\n\n\n\n\n\n\n\nWith the exception of non-derogable human rights (international conventions class the right to life, the right to be free from slavery, the right to be free from torture and the right to be free from retroactive application of penal laws as non-derogable), the UN recognises that human rights can be limited or even pushed aside during times of national emergency – although the emergency must be actual, affect the whole population and the threat must be to the very existence of the nation. The declaration of emergency must also be a last resort and a temporary measure. United Nations. The Resource\n\nRights that cannot be derogated for reasons of national security in any circumstances are known as peremptory norms or jus cogens. Such United Nations Charter obligations are binding on all states and cannot be modified by treaty. \n\nExamples of national security being used to justify human rights violations include the Japanese American internment during World War II,[135] Stalin's Great Purge,[136] and the modern-day abuses of terror suspects rights by some countries, often in the name of the War on Terror.",
                93,
                35,
                group6));

            group6.Items.Add(new SampleDataItem("Interview-Group-6-Item3",
                "Moral universalism",
                "",
                "Assets/HubPageImages/HubPageImage19.png",
                "Moral universalism (also called moral objectivism or universal morality) is the meta-ethical position that some system of ethics, or a universal ethic, applies universally, that is, for all similarly situated individuals",
                "\n\n\n\n\n\n\n\n\n\nMoral universalism (also called moral objectivism or universal morality) is the meta-ethical position that some system of ethics, or a universal ethic, applies universally, that is, for all similarly situated individuals, regardless of culture, race, sex, religion, nationality, sexuality, or any other distinguishing feature. Moral universalism is opposed to moral nihilism and moral relativism. However, not all forms of moral universalism are absolutist, nor are they necessarily value monist; many forms of universalism, such as utilitarianism, are non-absolutist, and some forms, such as that of Isaiah Berlin, may be value pluralist.\n\nIn addition to the theories of moral realism, moral universalism includes other cognitivist moral theories such as the subjectivist theories Ideal observer theory and the Divine command theory, and also the non-cognitivist moral theory universal prescriptivism.",
                93,
                34,
                group6));
            this.AllGroups.Add(group6);


        }
    }
}
