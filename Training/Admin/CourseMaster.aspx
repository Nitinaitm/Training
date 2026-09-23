                <!-- Remarks -->

                <div class="col-lg-6 mb-3">

                    <label class="form-label">
                        Remarks

                    </label>

                    <asp:TextBox
                        ID="txtRemarks"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="2">
                    </asp:TextBox>

                </div>

                <!-- Buttons -->

                <div class="col-lg-12 mt-3">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save"
                        CssClass="btn btn-success"
                        Width="120px"
                        ValidationGroup="SaveGroup"
                        OnClick="btnSave_Click" />

                    &nbsp;

                    <asp:Button
                        ID="btnUpdate"
                        runat="server"
                        Text="Update"
                        CssClass="btn btn-warning"
                        Width="120px"
                        Visible="false"
                        ValidationGroup="SaveGroup"
                        OnClick="btnUpdate_Click" />

                    &nbsp;

                  

                    <asp:Button
                        ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="btn btn-secondary"
                        Width="120px"
                        CausesValidation="false"
                        OnClick="btnClear_Click" />

                </div>

                <!-- Message -->

                <div class="col-lg-12 mt-3">

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Font-Bold="true"
                        Font-Size="14px">
                    </asp:Label>

                </div>

            </div>

        </div>

        <div class="main-card">

            <div class="row">

                <div class="col-lg-4 mb-3">

                    <label class="form-label">Search Course</label>

                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" placeholder="Enter Course Name"></asp:TextBox>

                </div>

                <div class="col-lg-8 text-end mt-4">

                    <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success" CausesValidation="false" OnClick="btnExportExcel_Click" />

                </div>

            </div>

            <div class="table-responsive">

                <asp:GridView ID="gvCourse" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover table-striped" Width="100%" DataKeyNames="CourseID" AllowPaging="true" PageSize="20" AllowSorting="true" OnPageIndexChanging="gvCourse_PageIndexChanging" OnSorting="gvCourse_Sorting" OnRowCommand="gvCourse_RowCommand">

                    <HeaderStyle CssClass="table-dark" />

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">

                            <ItemTemplate>

                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>

                            <ItemStyle Width="70px" HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:BoundField DataField="CourseName" HeaderText="Course Name" />

                        <asp:BoundField DataField="CourseCategory" HeaderText="Category" />

                        <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />

                        <asp:TemplateField HeaderText="Edit">

                            <ItemStyle Width="70px" HorizontalAlign="Center" />

                            <ItemTemplate>

                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm" Text="Edit" CommandName="EditRecord" CommandArgument='<%# Eval("CourseID") %>' CausesValidation="false"></asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Delete">

                            <ItemStyle Width="80px" HorizontalAlign="Center" />

                            <ItemTemplate>

                                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm" Text="Delete" CommandName="DeleteRecord" CommandArgument='<%# Eval("CourseID") %>' CausesValidation="false" OnClientClick="return confirm('Are you sure you want to delete this Course?');"></asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                    <EmptyDataTemplate>

                        <div class="text-center p-3">
                            No Course Found.

                        </div>

                    </EmptyDataTemplate>

                    <PagerStyle CssClass="table-secondary" HorizontalAlign="Center" />

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>
