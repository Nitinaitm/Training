(function () {
    function addEmployeeEditButtons() {
        var grid = document.getElementById('gvEmployee');
        if (!grid) return;
        var rows = grid.getElementsByTagName('tr');
        if (!rows.length) return;
        var header = rows[0];
        if (!header.querySelector('.employee-edit-header')) {
            var th = document.createElement('th'); th.className = 'employee-edit-header'; th.innerHTML = 'Action'; header.appendChild(th);
        }
        for (var i = 1; i < rows.length; i++) {
            var row = rows[i];
            if (!row.cells || row.cells.length < 2 || row.querySelector('.employee-edit-link')) continue;
            var empID = row.cells[1].innerText.trim();
            if (!empID || empID === '&nbsp;') continue;
            var cell = row.insertCell(-1); cell.style.textAlign = 'center';
            cell.innerHTML = '<button type="button" class="btn btn-sm btn-primary employee-edit-link" data-empid="' + encodeURIComponent(empID) + '"><i class="fa fa-edit"></i> Edit</button>';
        }
    }
    function openEmployeeEditor(empID) {
        var modalId = 'employeeEditModal'; var existing = document.getElementById(modalId); if (existing) existing.remove();
        var html = '<div class="modal fade" id="' + modalId + '" tabindex="-1" aria-hidden="true"><div class="modal-dialog modal-lg modal-dialog-centered"><div class="modal-content"><div class="modal-header bg-primary text-white"><h5 class="modal-title">Edit Employee</h5><button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button></div><div class="modal-body p-0" style="height:520px;overflow:hidden;"><iframe src="EditEmployee.aspx?EmpID=' + empID + '" style="width:100%;height:100%;border:0;"></iframe></div></div></div></div>';
        document.body.insertAdjacentHTML('beforeend', html);
        var modalElement = document.getElementById(modalId);
        if (window.bootstrap && window.bootstrap.Modal) new window.bootstrap.Modal(modalElement).show(); else { modalElement.style.display = 'block'; modalElement.classList.add('show'); }
    }
    document.addEventListener('click', function (e) { var button = e.target.closest ? e.target.closest('.employee-edit-link') : null; if (!button) return; e.preventDefault(); openEmployeeEditor(button.getAttribute('data-empid')); });
    function init() { addEmployeeEditButtons(); }
    window.addEventListener('load', init);
    if (typeof Sys !== 'undefined' && Sys.Application) Sys.Application.add_load(init);
})();