<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Categories.aspx.vb" Inherits="IOF.Web.Categories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="scripts/bootstrap-treeview/bootstrap-treeview.min.css" />

    <style>
        .tree-container {
            max-height: 600px;
            overflow-y: scroll;
            border: solid 1px #ddd;
            border-radius: 4px;
        }

            .tree-container .list-group-item {
                border-left: none;
                border-right: none;
            }

                .tree-container .list-group-item:first-child {
                    border-top: none;
                }

                .tree-container .list-group-item:last-child {
                    border-bottom: none;
                }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Manage Categories</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <asp:PlaceHolder runat="server" ID="phEditForms" Visible="false">
                <div class="form-inline parent-form" style="margin-bottom: 10px;">
                    <div class="form-group">
                        <label class="text-right" style="width: 60px;">Parent:</label>
                        <input type="text" class="category-number form-control" style="width: 80px;" />
                    </div>
                    <div class="form-group">
                        <input type="text" class="category-name form-control" style="width: 400px;" maxlength="50" />
                    </div>
                    <div style="display: inline-block;">
                        <span><a href="#" class="button" data-command="add" title="Add"><img src="https://ssel-apps.eecs.umich.edu/static/images/add.png" alt="Add" /></a></span>
                        <span style="display: none;"><a href="#" class="button" data-command="modify" title="Modify"><img src="https://ssel-apps.eecs.umich.edu/static/images/save.png" alt="Modify" /></a></span>
                        <span style="display: none;">| <a href="#" class="button" data-command="delete" title="Delete"><img src="https://ssel-apps.eecs.umich.edu/static/images/delete.png" alt="Delete" /></a></span>
                    </div>
                </div>

                <div class="form-inline child-form" style="margin-bottom: 10px;">
                    <div class="form-group">
                        <label class="text-right" style="width: 60px;">Child:</label>
                        <input type="text" class="category-number form-control" style="width: 80px;" />
                    </div>
                    <div class="form-group">
                        <input type="text" class="category-name form-control" style="width: 400px;" maxlength="50" />
                    </div>
                    <div style="display: inline-block;">
                        <span><a href="#" class="button" data-command="add" title="Add"><img src="https://ssel-apps.eecs.umich.edu/static/images/add.png" alt="Add" /></a></span>
                        <span style="display: none;"><a href="#" class="button" data-command="modify" title="Modify"><img src="https://ssel-apps.eecs.umich.edu/static/images/save.png" alt="Modify" /></a></span>
                        <span style="display: none;">| <a href="#" class="button" data-command="delete" title="Delete"><img src="https://ssel-apps.eecs.umich.edu/static/images/delete.png" alt="Delete" /></a></span>
                    </div>
                </div>
            </asp:PlaceHolder>

            <div class="row">
                <div class="col-md-6">
                    <div class="error" style="display: none;">
                        <div class="alert alert-danger" role="alert">
                        </div>
                    </div>
                    <div class="tree-container">
                        <div id="category-tree">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="scripts/bootstrap-treeview/bootstrap-treeview.min.js"></script>
    <script>
        var treeView = null;

        function toggleForm(form, disabled, node) {
            if (node) {
                $(".category-number", form).val(node.categoryNumber).prop("disabled", disabled);
                $(".category-name", form).val(node.categoryName).prop("disabled", disabled);
                $(".button[data-command='add']", form).parent().hide();
                $(".button[data-command='modify']", form).parent().toggle(!disabled);
                $(".button[data-command='delete']", form).parent().toggle(!disabled);
            } else {
                $(".category-number", form).val("").prop("disabled", disabled);
                $(".category-name", form).val("").prop("disabled", disabled);
                $(".button[data-command='add']", form).parent().toggle(!disabled);
                $(".button[data-command='modify']", form).parent().hide();
                $(".button[data-command='delete']", form).parent().hide();
            }
        }

        function handleUnselect() {
            toggleForm($(".parent-form"), false);
            toggleForm($(".child-form"), true);
        }

        function handleSelect(node) {
            if (node.isParent) {
                toggleForm($(".parent-form"), false, node);
                toggleForm($(".child-form"), false);
            } else {
                var pnode = treeView.data("treeview").getParent(node);
                toggleForm($(".parent-form"), true, pnode);
                toggleForm($(".child-form"), false, node);
            }
        }

        function loadTreeView(data) {
            return $("#category-tree").treeview({
                "data": data,
                "levels": 1
            }).on("nodeSelected", function (e, node) {
                handleSelect(node);
            }).on("nodeUnselected", function (e, node) {
                handleUnselect();
            });
        }

        function showError(err) {
            if (err)
                $(".error .alert").html(err.message);
            else
                $(".error .alert").html("A server error occurred.");

            $(".error").show();
        }

        handleUnselect();

        $.ajax({
            "url": "ajax/category.ashx",
            "method": "GET",
        }).done(function (data) {
            treeView = loadTreeView(data);
            loaded = true;
        }).fail(function (jqXHR) {
            showError(jqXHR.responseJSON);
        });

        $(".button").on("click", function (e) {
            e.preventDefault();

            if (!treeView) return;

            var cmd = $(this).data("command");

            if (cmd === "add") {
                var num, name;

                var selectedNodes = treeView.data("treeview").getSelected();
                var parentId;

                if (selectedNodes.length === 0) {
                    // nothing is selected so we are adding a parent (parentId = 0)
                    num = $(".parent-form .category-number").val();
                    name = $(".parent-form .category-name").val();
                    parentId = 0;
                } else {
                    // something is selected so we are adding a child (parentId = selectedNodes[0].categoryId)
                    num = $(".child-form .category-number").val();
                    name = $(".child-form .category-name").val();
                    parentId = selectedNodes[0].categoryId;
                }

                $(".error").hide();

                treeView = null;

                $.ajax({
                    "url": "ajax/category.ashx",
                    "method": "POST",
                    "data": { "command": "add", "parentId": parentId, "categoryName": name, "categoryNumber": num }
                }).done(function (data) {
                    treeView = loadTreeView(data);
                    if (selectedNodes.length > 0) {
                        $(".child-form .category-number").val("");
                        $(".child-form .category-name").val("");
                        treeView.data("treeview").expandNode(selectedNodes[0].nodeId, { "silent": false });
                        treeView.data("treeview").selectNode(selectedNodes[0].nodeId, { "silent": false });
                    } else {
                        $(".parent-form .category-number").val("");
                        $(".parent-form .category-name").val("");
                    }
                }).fail(function (jqXHR) {
                    showError(jqXHR.responseJSON);
                });
            } else if (cmd === "modify") {
                // modify
                var num, name;

                var selectedNodes = treeView.data("treeview").getSelected();

                // something must be selected to modify
                if (selectedNodes.length === 0) return;

                var isParent = selectedNodes[0].isParent;
                var isExpanded = selectedNodes[0].state.expanded;
                var categoryId = selectedNodes[0].categoryId;

                if (isParent) {
                    num = $(".parent-form .category-number").val();
                    name = $(".parent-form .category-name").val();
                } else {
                    num = $(".child-form .category-number").val();
                    name = $(".child-form .category-name").val();
                }

                $(".error").hide();

                treeView = null;

                $.ajax({
                    "url": "ajax/category.ashx",
                    "method": "POST",
                    "data": { "command": "modify", "categoryId": categoryId, "categoryName": name, "categoryNumber": num }
                }).done(function (data) {
                    treeView = loadTreeView(data);

                    if (isParent) {
                        if (isExpanded)
                            treeView.data("treeview").expandNode(selectedNodes[0].nodeId, { "silent": false });
                    } else {
                        treeView.data("treeview").revealNode(selectedNodes[0].nodeId, { "silent": false });
                    }

                    treeView.data("treeview").selectNode(selectedNodes[0].nodeId, { "silent": false });
                }).fail(function (jqXHR) {
                    showError(jqXHR.responseJSON);
                });
            } else if (cmd === "delete") {
                // delete
                var selectedNodes = treeView.data("treeview").getSelected();

                if (selectedNodes.length > 0) {
                    var pnode = treeView.data("treeview").getParent(selectedNodes[0]);
                    var categoryId = selectedNodes[0].categoryId;

                    if (confirm('Are you sure you want to delete this category?')) {
                        $(".error").hide();

                        treeView = null;

                        $.ajax({
                            "url": "ajax/category.ashx",
                            "method": "POST",
                            "data": { "command": "delete", "categoryId": categoryId }
                        }).done(function (data) {
                            treeView = loadTreeView(data);

                            if (pnode)
                                treeView.data("treeview").expandNode(pnode.nodeId, { "silent": false });

                            handleUnselect();

                        }).fail(function (jqXHR) {
                            showError(jqXHR.responseJSON);
                        });
                    }
                }
            }
        });
    </script>
</asp:Content>
