$(function () {
    let lobIndex = 0;
    function collectLobs() {
        const lobs = [];
        document.querySelectorAll("#lobContainer .lob-block").forEach(block => {
            const index = block.dataset.index;
            lobs.push({
                Geo: block.querySelector(`[name="geo${index}"]`).value,
                Site: block.querySelector(`[name="site${index}"]`).value,
                Name: block.querySelector(`[name="name${index}"]`).value,
                BillingModel: block.querySelector(`[name="billingModel${index}"]`).value,
                ProjectId: block.querySelector(`[name="projectId${index}"]`).value,
                TrainingWk: block.querySelector(`[name="trainingWk${index}"]`).value,
                NestingWk: block.querySelector(`[name="nestingWk${index}"]`).value,
                LearningWk: block.querySelector(`[name="learningWk${index}"]`).value
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
                window.location.href = `/Plan/ViewPlan?planId=${response.planId}&accountName=${encodeURIComponent(formData.Account)}&planName=${encodeURIComponent(formData.Name)}`
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
                                            <label class="form-label">Geo (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                            <select class="form-control form-select" name="geo${lobIndex}" required>
                                            <option value="">Select</option>
                                            <option>Geo 1</option>
                                            <option>Geo 2</option>
                                            </select>
                                          </div>
                                           <div class="col-md-3">
                                                    <label class="form-label">Site (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                            <input type="text" class="form-control" name="site${lobIndex}" required>
                                          </div>
                                            <div class="col-md-3">
                                                    <label class="form-label">Name (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                            <input type="text" class="form-control" name="name${lobIndex}" required>
                                          </div>

                                     <div class="col-md-3">
                                    <label class="form-label">Billing Model (LOB-${lobIndex})<span class="text-danger">*</span></label>
                                    <select class="form-control form-select" name="billingModel${lobIndex}" required>
                                      <option value="">Select Model</option>
                                       <option value="FTE">FTE</option>
                                       <option value="Transaction">Transaction</option>
                                    </select>
                                  </div>
                                 

                                </div>
                                <div class="row"> 
                                     <div class="col-md-3">
                                            <label class="form-label">Project ID<span class="text-danger">*</span></label>
                                    <input type="text" class="form-control" name="projectId${lobIndex}" required>
                                  </div>
                                
                                   <div class="col-md-3">
                                     <span>Training WK<span class="text-danger">*</span></span>
                                     <select class="form-control form-select" name="trainingWk${lobIndex}" style="margin-top: 7px;" required>
                                       <option value="0">0</option>
                                       <option value="1">1</option>
                                       <option value="2">2</option>
                                       <option value="3">3</option>
                                       <option value="4">4</option>
                                       <option value="5">5</option>
                                       <option value="6">6</option>
                                       <option value="7">7</option>
                                       <option value="8">8</option>
                                       <option value="9">9</option>
                                       <option value="10">10</option>
                                       <option value="11">11</option>
                                       <option value="12">12</option>
                                    </select>
                                  </div>
                                   <div class="col-md-3">
                                 <span>Nesting WK<span class="text-danger">*</span></span>
                                     <select class="form-control form-select" name="nestingWk${lobIndex}" style="margin-top: 7px;"  required>
                                       <option value="0">0</option>
                                       <option value="1">1</option>
                                       <option value="2">2</option>
                                       <option value="3">3</option>
                                       <option value="4">4</option>
                                       <option value="5">5</option>
                                       <option value="6">6</option>
                                       <option value="7">7</option>
                                       <option value="8">8</option>
                                       <option value="9">9</option>
                                       <option value="10">10</option>
                                       <option value="11">11</option>
                                       <option value="12">12</option>
                                    </select>
                                  </div>
                                    <div class="col-md-2">
                                 <span>Learning curve WK<span class="text-danger">*</span></span>
                                     <select class="form-control form-select" name="learningWk${lobIndex}" style="margin-top: 7px;"  required>
                                       <option value="0">0</option>
                                       <option value="1">1</option>
                                       <option value="2">2</option>
                                       <option value="3">3</option>
                                       <option value="4">4</option>
                                       <option value="5">5</option>
                                       <option value="6">6</option>
                                       <option value="7">7</option>
                                       <option value="8">8</option>
                                       <option value="9">9</option>
                                       <option value="10">10</option>
                                       <option value="11">11</option>
                                       <option value="12">12</option>
                                    </select>
                                  </div>
                                  <div class="col-md-1 d-flex align-items-end">
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