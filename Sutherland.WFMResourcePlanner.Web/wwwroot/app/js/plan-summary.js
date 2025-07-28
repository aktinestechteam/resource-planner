$(function () {
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
              <a href="/Plan/ViewPlan/${p.planId}" class="btn btn-sm btn-primary">View</a>
			  <a href="/Plan/Edit/${p.planId}" class="btn btn-sm btn-secondary">Edit</a>
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
										<a href="/Plan/ViewPlan/${plan.planId}" class="btn btn-sm btn-primary">View</a>
										<a href="/Plan/Edit/${plan.planId}" class="btn btn-sm btn-secondary">Edit</a>
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





});
