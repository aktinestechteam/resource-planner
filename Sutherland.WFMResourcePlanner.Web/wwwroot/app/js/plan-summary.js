$(function () {
    let copyModal = new bootstrap.Modal(document.getElementById('copyPlanModal'));
    loadPlans();
    function loadPlans() {

        let isListView = false;

        $('#toggleViewBtnList').click(function () {
            isListView = !isListView;

            $('#listView').toggleClass('d-none', !isListView);
            $('#cardView').toggleClass('d-none', isListView);
            $('#toggleViewBtnList').addClass('d-none');
            $('#toggleViewBtnCards').removeClass('d-none');
        });
        $('#toggleViewBtnCards').click(function () {
            isListView = !isListView;
            $('#listView').toggleClass('d-none', !isListView);
            $('#cardView').toggleClass('d-none', isListView);
            $('#toggleViewBtnCards').addClass('d-none');
            $('#toggleViewBtnList').removeClass('d-none');
        });


        $.get('/plan/GetAllPlans', function (data) {
            if (!data || data.length === 0) {
                $('#plansContainer').html('<div class="col-12"><div class="no-data">🚫 No plans available.</div></div>');
                return;
            }
            renderList(data);
            renderCards(data);
        }).fail(function () {
            $('#plansContainer').html('<div class="col-12"><div class="no-data text-danger">❌ Failed to load plans.</div></div>');
        });

        function renderList(plans) {
            let html = '';
            plans.forEach(p => {
                html += `<tr>
          <td>${p.name}</td>
          <td>${p.vertical}</td>
          <td>${p.account}</td>
          <td>${p.geo}</td>
          <td>${p.site}</td>
          <td>${formatDate(p.planFrom)} - ${formatDate(p.planTo)}</td>
          <td>
            <a href="/Plan/ViewPlan?planId=${p.planId}&accountName=${encodeURIComponent(p.account)}&planName=${encodeURIComponent(p.name)}" class="btn btn-sm btn-primary">View</a>
			<a href="/Plan/ViewPlan?planId=${p.planId}&accountName=${encodeURIComponent(p.account)}&planName=${encodeURIComponent(p.name)}" class="btn btn-sm btn-secondary">Edit</a>
            <button class="btn btn-sm btn-outline-success copy-plan-btn" data-plan-id=${p.planId} data-plan-name="${p.name}">Create Copy</button>
            </td>
        </tr>`;
            });
            $('#listContainer').html(html);
        }
        function renderCards(plans) {
            let html = '';
            $.each(plans, function (i, plan) {
                html += `
						<div class="col-md-4">
							<div class="card plan-card h-100">
								<div class="card-body d-flex flex-column justify-content-between">
									<div>
										<div class="plan-header">${plan.name}</div>
										<div class="plan-meta">
											<div><strong>Vertical:</strong> ${plan.vertical}</div>
											<div><strong>Account:</strong> ${plan.account}</div>
											<div><strong>Geo:</strong> ${plan.geo}</div>
											<div><strong>Site:</strong> ${plan.site}</div>
											<div><strong>Duration:</strong> ${formatDate(plan.planFrom)} → ${formatDate(plan.planTo)}</div>
										</div>
									</div>
									<div class="mt-3 plan-actions">
										<a href="/Plan/ViewPlan?planId=${plan.planId}&accountName=${encodeURIComponent(plan.account)}&planName=${encodeURIComponent(plan.name)}" class="btn btn-sm btn-primary">View</a>
										<a href="/Plan/ViewPlan?planId=${plan.planId}&accountName=${encodeURIComponent(plan.account)}&planName=${encodeURIComponent(plan.name)}" class="btn btn-sm btn-secondary">Edit</a>
                                        <button class="btn btn-sm btn-outline-success copy-plan-btn" data-plan-id=${plan.planId} data-plan-name="${plan.name}">Create Copy</button>
									</div>
								</div>
							</div>
						</div>`;
            });

            $('#plansContainer').html(html);
        }
    }

    function formatDate(dateStr) {
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
    }

    

    // Open modal and populate plan ID
    $(document).on('click', '.copy-plan-btn', function () {

        const planId = $(this).data('plan-id');
        const planName = $(this).data('plan-name');
        $('#sourcePlanId').val(planId);
        $('#copyPlanModalLabel').text("Create a Copy of Plan - " + planName)
       
        $('#newPlanName').val('');
        copyModal.show();
    });

    // Handle copy form submit
    $('#copyPlanForm').on('submit', function (e) {
        e.preventDefault();

        const sourcePlanId = $('#sourcePlanId').val();
        const newPlanName = $('#newPlanName').val().trim();

        if (!newPlanName) {
            alert('Please enter a plan name.');
            return;
        }

        $.ajax({
            url: '/Plan/CopyPlan',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                sourcePlanId: sourcePlanId,
                newPlanTitle: newPlanName
            }),
            success: function (response) {
                copyModal.hide();
                alert('Plan copied successfully!');
                location.reload();
            },
            error: function () {
                alert('Failed to copy plan. Please try again.');
            }
        });
    });



});
