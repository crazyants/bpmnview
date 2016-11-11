﻿// 
// The MIT License (MIT)
//
// Copyright (c) 2016 Boris Zinchenko
// mail: boris.zinchenko@caseagile.com
// web: http://www.caseagile.com
// code: https://github.com/bzinchenko/bpmnview
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BPMN.View
{
  public partial class ElementsForm : Form
  {
    private Model model;

    public ElementsForm(Model mdl)
    {
      InitializeComponent();
      model = mdl;
    }

    private void ElementsForm_Load(object sender, EventArgs e)
    {
      if (model != null)
        gridElements.DataSource = ElementsTable(model.Elements);
    }

    private void gridElements_SelectionChanged(object sender, EventArgs e)
    {
      ViewElement(true);
    }

    private void gridSubElements_SelectionChanged(object sender, EventArgs e)
    {
      //ViewElement(false);
    }

    private void ViewElement(bool primary) 
    {
      gridSubElements.DataSource = null;
      gridAttributes.DataSource = null;
      if(primary) gridProperties.DataSource = null;

      if (gridElements.SelectedRows.Count > 0)
      {
        string id = gridElements.SelectedRows[0].Cells["ID"].Value.ToString();
        Element el = ElementByID(id);
        if (el != null)
        {
          if (primary)
          {
            if (el.Elements != null)
            {
              List<Element> elm = new List<Element>();
              foreach (var elt in el.Elements)
              {
                if (elt.Value != null)
                  elm.AddRange(elt.Value);
              }
              gridSubElements.DataSource = ElementsTable(elm);
            }
          }

          if (el.Properties != null)
          {
            Dictionary<string, string> props = new Dictionary<string, string>();
            foreach (var prop in el.Properties)
            {
              string propList = "";
              foreach (string pr in prop.Value)
              {
                if (!string.IsNullOrEmpty(propList))
                  propList += ", ";
                propList += pr;
              }
              props.Add(prop.Key, propList);
            }
          }

          gridAttributes.DataSource = StringTable(el.Attributes);
        }
      }
    }

    private DataTable ElementsTable(IEnumerable<Element> elements)
    {
      DataTable table = new DataTable();
      table.Columns.Add("Name", typeof(string));
      table.Columns.Add("TypeName", typeof(string));
      table.Columns.Add("ID", typeof(string));
      table.Columns.Add("ParentID", typeof(string));

      if (elements == null) 
        return null;

      table.BeginLoadData();
      foreach (Element el in elements)
      {
        if (el.TypeName.Contains("BPMN")) continue;
        DataRow row = table.NewRow();
        if (el.Attributes.ContainsKey("name"))
          row["Name"] = el.Attributes["name"];
        row["TypeName"] = el.TypeName;
        if (el.Attributes.ContainsKey("id"))
          row["ID"] = el.Attributes["id"];
        row["ParentID"] = el.ParentID;
        table.Rows.Add(row);
      }
      table.EndLoadData();
      return table;
    }

    private DataTable StringTable(Dictionary<string, string> elements)
    {
      DataTable table = new DataTable();
      table.Columns.Add("Name", typeof(string));
      table.Columns.Add("Value", typeof(string));

      if (elements == null)
        return null;

      table.BeginLoadData();
      foreach (var el in elements)
      {
        DataRow row = table.NewRow();
        row["Name"] = el.Key;
        row["Value"] = el.Value;
        table.Rows.Add(row);
      }
      table.EndLoadData();
      return table;
    }

    public Element ElementByID(string id)
    {
      if (model != null && !string.IsNullOrEmpty(id))
      {
        foreach (Element element in model.Elements)
          if (element.Attributes.ContainsKey("id") &&
            id.Equals(element.Attributes["id"])) return element;
      }
      return null;
    }

  }
}