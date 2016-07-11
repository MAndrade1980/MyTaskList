<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MyTaskList._Default" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>My Task List</h1>
        <div>

            
            <asp:Panel runat="server" ID="AddPanel" DefaultButton="AddNewTaskButton">
                <asp:TextBox runat="server" ID="TaskName" MaxLength="255"></asp:TextBox>
                <asp:DropDownList runat="server" ID="ColorList" CssClass="LST">
                    <asp:ListItem Text="White" Value="White"></asp:ListItem>
                    <asp:ListItem Text="Gray" Value="LightGray"></asp:ListItem>
                    <asp:ListItem Text="Beige" Value="Beige"></asp:ListItem>
                    <asp:ListItem Text="Pink" Value="Pink"></asp:ListItem>
                    <asp:ListItem Text="Green" Value="LightGreen"></asp:ListItem>
                    <asp:ListItem Text="Blue" Value="LightBlue"></asp:ListItem>
                </asp:DropDownList>
                <asp:LinkButton runat="server" ID="AddNewTaskButton" Text="Add New Task" 
                    OnClick="AddNewTaskButton_Click"></asp:LinkButton>
            </asp:Panel>

            <asp:Label runat="server" ID="StatusLabel"></asp:Label>
            <br />

            <asp:RadioButton runat="server" ID="ActiveOnly" CssClass="RadioButtons" Text="Active Items" GroupName="StatusSearch" Checked="true" AutoPostBack="true" OnCheckedChanged="ActiveOnly_CheckedChanged" />&nbsp;
            <asp:RadioButton runat="server" ID="CompletedOnly" CssClass="RadioButtons" Text="Completed Items" GroupName="StatusSearch" AutoPostBack="true" OnCheckedChanged="CompletedOnly_CheckedChanged" />&nbsp;
            <asp:RadioButton runat="server" ID="BothStatus" CssClass="RadioButtons" Text="Both" GroupName="StatusSearch" AutoPostBack="true" OnCheckedChanged="BothStatus_CheckedChanged" />

            <asp:GridView runat="server" ID="TasksGrid" ShowHeader="false" DataKeyNames="Id" 
                AutoGenerateColumns="false" 
                CssClass="drag_drop_grid GridSrc"  
                OnRowDataBound="TasksGrid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Id" ItemStyle-CssClass="IdColumn" />
                    
                    <asp:TemplateField ItemStyle-CssClass="ColumnCenter" ItemStyle-Width="30">

                        <ItemTemplate>
                            <asp:hiddenfield runat="server" id="TaskIdHiddenField" Value='<%# Eval("Id") %>' />
                            <asp:CheckBox runat="server" ID="Completed" 
                                Checked='<%#Eval("Status").ToString() == "Completed" ? true : false%>' 
                                Autopostback="true" OnCheckedChanged="Completed_CheckedChanged" />
                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:BoundField DataField="Name" ItemStyle-CssClass="ColumnLeft" 
                        ItemStyle-Width="420" />

                    <asp:TemplateField ItemStyle-CssClass="ColumnCenter" ItemStyle-Width="50">
                        <ItemTemplate>
                            <asp:hiddenfield runat="server" id="TaskIdDelete" Value='<%# Eval("Id") %>' />
                            <asp:ImageButton runat="server" ID="DeleteTask" 
                                ImageUrl="~/Images/Delete-Icon.png" 
                                OnClick="DeleteTask_Click" CssClass="GridButton" 
                                OnClientClick="if (!confirm('Are you sure you want delete?')) return false;" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
            
            
        </div>
    </div>

<script type="text/javascript">
    $(function () {
        $(".drag_drop_grid").sortable({
            items: 'tr',
            cursor: 'crosshair',
            axis: 'y',
            dropOnEmpty: true,
            update: function (event, ui) {

                var data = '';
                var cols = $('.IdColumn');
                for (i = 0; i < cols.length; i++) {
                    data = data + '|' + cols[i].innerHTML;
                }
                
                // POST to server using $.post or $.ajax
                $.ajax({
                    data: data,
                    type: 'POST',
                    url: '/default.aspx?action=sortItems&data=' + data
                });

                // Update status label
                $("span[id$='StatusLabel']").text('Task items sorted successfully!');
                $("span[id$='StatusLabel']").css("color","green");


            }
        });

    });
</script>

</asp:Content>
