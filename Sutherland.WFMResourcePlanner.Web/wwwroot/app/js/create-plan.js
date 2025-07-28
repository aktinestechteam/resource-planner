$(function () {
    let lobIndex = 0;
    function collectLobs() {
        const lobs = [];
        document.querySelectorAll("#lobContainer .lob-block").forEach(block => {
            const index = block.dataset.index;
            lobs.push({
                Name: block.querySelector(`[name="name${index}"]`).value,
                BillingModel: block.querySelector(`[name="billingModel${index}"]`).value,
                ProjectId: block.querySelector(`[name="projectId${index}"]`).value
            });
        });
        return lobs;
    }
    $("#planForm").on("submit", function (e) {
        e.preventDefault();
        const formData = {
            Name: $("[name='Name']").val(),
            Vertical: $("[name='Vertical']").val(),
            Account: $("[name='Account']").val(),
            Geo: $("[name='Geo']").val(),
            Site: $("[name='Site']").val(),
            BusinessUnit: $("[name='BusinessUnit']").val(),
            SOTracker: $("#soTracker").is(":checked"),
            AssumptionSheet: $("#assumptionSheet").is(":checked"),
            WeekStart: $("[name='WeekStart']").val(),
            PlanFrom: $("[name='PlanFrom']").val(),
            PlanTo: $("[name='PlanTo']").val(),
            LOBs: collectLobs()
        };

        $.ajax({
            url: "/plan/create",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                alert("Plan saved successfully!");
                window.location.href = "/Plan/ViewPlan/" + response.planId;
            },
            error: function (xhr) {
                console.error("Save failed:", xhr.responseText);
                alert("Error saving plan.");
            }
        });
    });
    $('.plan-add-lob').on('click', function (e) {
        e.preventDefault();
        lobIndex++;
        const lobHtml = `
                              <div class="lob-block border-top pt-3 mt-3" data-index="${lobIndex}">
                                <div class="row">
                                         <div class="col-md-3">
                                                    <label class="form-label">Name (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                            <input type="text" class="form-control" name="name${lobIndex}" required>
                                          </div>
                                  <div class="col-md-3">
                                            <label class="form-label">Billing Model (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                    <select class="form-select" name="billingModel${lobIndex}" required>
                                      <option value="">Select Model</option>
                                       <option value="FTE">FTE</option>
                                       <option value="Transaction">Transaction</option>
                                    </select>
                                  </div>
                                  <div class="col-md-3">
                                            <label class="form-label">Project ID (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                    <input type="text" class="form-control" name="projectId${lobIndex}" required>
                                  </div>
                                  <div class="col-md-2 d-flex align-items-end">
                                    <button type="button" class="btn btn-outline-danger plan-remove-lob">
                                      <i class="fa-solid fa-trash"></i>
                                    </button>
                                  </div>
                                </div>
                              </div>
                            `;
        document.getElementById("lobContainer").insertAdjacentHTML("beforeend", lobHtml);
    });
    $(document).on('click', '.plan-remove-lob', function (e) {
        e.preventDefault();
        let button = this;
        const lobBlock = button.closest(".lob-block");
        if (lobBlock) lobBlock.remove();
    });
});