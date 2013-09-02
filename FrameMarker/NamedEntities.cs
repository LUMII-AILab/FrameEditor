using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.IO;

namespace FrameMarker
{
    public partial class frmNamedEntities : Form
    {
        public Dictionary<int, NamedEntity> NamedEntities; 
        public frmNamedEntities(/*SemanticDatabase*/Document db, NamedEntity selected)
        {
            InitializeComponent();

            int selIndex = db.NamedEntities.Values.ToList().IndexOf(selected);
            NamedEntities = db.NamedEntities.SerializeClone();
            
            RefreshNamedEntityList(NamedEntities.Values.ToList());

            lvNamedEntities.Items[selIndex].Selected = true;
                                    
            lvNamedEntities.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            try
            {
                this.LoadCategories(File.ReadAllText("categories.csv"));
            }
            catch (Exception ex)
            {
                this.LoadCategories(DataResource.Categories);
            }
        }

	private void LoadCategories(string text)
	{
	    var categories = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
	    this.cbCategory.Items.Clear();
	    this.cbCategory.Items.AddRange((object [])categories);
	}
        
        void RefreshNamedEntityList(List<NamedEntity> namedEntities)
        {
            lvNamedEntities.Items.Clear();
            foreach (var entity in namedEntities)
            {
                var aliasStr = string.Join(", ", entity.AliasSet);
                if (entity.AliasSet.Count == 0)
                {
                    aliasStr = "-       ";
                }
                var it = new ListViewItem(new[] { entity.ID.ToString().PadRight(4), entity.Name, entity.Type, aliasStr });
                it.Tag = entity;
                lvNamedEntities.Items.Add(it);
            }

            txtName.Clear();
            cbCategory.Text = "";
            dgAlias.Rows.Clear();
        }

        NamedEntity GetSelectedEntity()
        {
            if (lvNamedEntities.SelectedItems.Count > 0)
            {
                var entity = lvNamedEntities.SelectedItems[0].Tag as NamedEntity;
                return entity;
            }
            return null;
        }

        private void lvNamedEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if(entity != null)
            {                
                txtName.Text = entity.Name;
                cbCategory.Text = entity.Type;
                                
                RefreshAliasList();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity != null)
            {
                entity.Name = txtName.Text;
                lvNamedEntities.Items[lvNamedEntities.SelectedIndices[0]].SubItems[1].Text = entity.Name;
            }
        }
        
        private void dgAlias_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                var entity = GetSelectedEntity();
                if (entity != null)
                {
                    if (dgAlias.SelectedCells.Count > 0)
                    {                        
                        if(!dgAlias.CurrentRow.IsNewRow)
                        {
                            int inx = dgAlias.CurrentRow.Index;
                            entity.AliasSet.Remove((dgAlias.CurrentCell.Value??"").ToString());
                            dgAlias.Rows.Remove(dgAlias.CurrentRow);                            
                            RefreshAliasList();
                            if(inx < dgAlias.Rows.Count )
                            {
                                dgAlias.CurrentCell = dgAlias.Rows[inx].Cells[0];    
                            }

                            lvNamedEntities.Items[lvNamedEntities.SelectedIndices[0]].SubItems[3].Text = string.Join(", ", entity.AliasSet);
                        }                        
                    }    
                }                
            }
        }

        private void dgAlias_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
        }

        private void dgAlias_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        void RefreshAliasList()
        {
            var entity = GetSelectedEntity();
            if (entity != null)
            {
                dgAlias.Rows.Clear();
                foreach (var alias in entity.AliasSet)
                {
                    dgAlias.Rows.Add(alias);
                }
            }
        }

        private void dgAlias_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity != null)
            {
                entity.AliasSet.Clear();
                for (int n = 0; n < dgAlias.Rows.Count; n++ )
                {
                    var val = dgAlias.Rows[n].Cells[0].Value;
                    if(val != null)
                    {
                        entity.AliasSet.Add(val.ToString());

                        lvNamedEntities.Items[lvNamedEntities.SelectedIndices[0]].SubItems[3].Text = string.Join(", ", entity.AliasSet);
                    }
                    
                }                    
            }
        }

        private void cbCategory_TextUpdate(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity != null)
            {
                entity.Type = cbCategory.Text;
                lvNamedEntities.Items[lvNamedEntities.SelectedIndices[0]].SubItems[2].Text = entity.Type;
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entity = GetSelectedEntity();
            if (entity != null)
            {
                entity.Type = cbCategory.Text;
                lvNamedEntities.Items[lvNamedEntities.SelectedIndices[0]].SubItems[2].Text = entity.Type;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var matchingEntities = NamedEntities.Values.Where(o => 
                o.Name.ToLowerInvariant().Contains(txtSearch.Text.Trim().ToLowerInvariant())).ToList();
            RefreshNamedEntityList(matchingEntities);
        }

        
    }
}
