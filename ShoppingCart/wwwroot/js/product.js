var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    console.log('hi');
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/product/getall'
        },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'author', "width": "20%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-info mx-2"><i class="bi bi-pencil-square"></i></a>
                        <a onClick="Delete('/admin/product/delete/${data}')" class="btn btn-warning mx-2"><i class="bi bi-trash3-fill"></i></a>
                    </div>`
                },
                "width": "15%"
            }

        ]
    })
}

function Delete(url) {
    Swal.fire({
        title:"Are you sure you want to delete this product?",
        showClass: {
            popup: `
      animate__animated
      animate__fadeInUp
      animate__faster
    `
        },
        hideClass: {
            popup: `
      animate__animated
      animate__fadeOutDown
      animate__faster
    `
        },
        text:'This will be a Permanent Delete',
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}