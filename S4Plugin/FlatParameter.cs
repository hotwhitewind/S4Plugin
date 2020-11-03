using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace S4Plugin
{
    public class FlatParameter
    {

        public string Name { get; set; }

        public string GUID { get; set; }

        public ParameterType DataType { get; set; }

        public bool Visible { get; set; }

        public List<BuiltInCategory> Categories { get; set; }

        public BuiltInParameterGroup ParameterGroup { get; set; }

        public bool InstanceBinding { get; private set; }

        public FlatParameter()
        {
            this.InstanceBinding = true;
        }

        public string GetParamString()
        {
            return string.Format("PARAM\t{0}\t{1}\t{2}\t\t{3}\t{4}", new object[]
            {
                this.GUID,
                this.Name,
                this.DataType.ParamToString(),
                1,
                this.Visible ? 1 : 0
            });
        }

        public Definition GetDefinition(DefinitionGroup group)
        {
            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(this.Name, this.DataType);
            externalDefinitionCreationOptions.Visible = true;
            Definition definition = group.Definitions.get_Item(this.Name);
            if (definition == null)
            {
                definition = group.Definitions.Create(externalDefinitionCreationOptions);
            }
            return definition;
        }

        public bool BindParameter(Document document)
        {
            DefinitionFile definitionFile = document.Application.OpenSharedParameterFile();

            DefinitionGroup definitionGroup = definitionFile.Groups.get_Item("BROWNIE");
            if (definitionGroup == null)
            {
                return false;
            }
            Definition definition = this.GetDefinition(definitionGroup);

            CategorySet categorySet = document.Application.Create.NewCategorySet();
            Category category = document.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms);
            categorySet.Insert(category);

            Binding binding;
            if (this.InstanceBinding)
            {
                binding = document.Application.Create.NewInstanceBinding(categorySet);
                document.ParameterBindings.Insert(definition, binding, BuiltInParameterGroup.PG_DATA);
            }
            else
            {
                binding = document.Application.Create.NewTypeBinding(categorySet);
            }
            bool flag = document.ParameterBindings.Insert(definition, binding);
            if (!flag)
            {
                flag = document.ParameterBindings.ReInsert(definition, binding);
            }
            return flag;
        }
    }
}
