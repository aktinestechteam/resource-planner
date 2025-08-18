(function ($) {
    $.luckySheetEnhancer = function (instance, options) {
        const settings = $.extend({
            protectedRows: { start: 0, end: 9 },
            customMenuItems: [],
            allowRenameSheet: false,
        }, options);

        const getContainer = () => instance?.config?.container || 'luckysheet';

        const blockDeleteProtectedRows = () => {
            const observer = new MutationObserver(() => {
                const menu = document.querySelector("#luckysheet-cols-menu");
                if (menu && !menu.querySelector(".custom-protection-applied")) {
                    const items = menu.querySelectorAll(".luckysheet-menus-item");
                    items.forEach((item) => {
                        const text = item.innerText.trim();
                        if (text === "Delete Row") {
                            const selected = instance.getRange()[0];
                            const r1 = selected.row[0];
                            const r2 = selected.row[1];
                            if (r1 <= settings.protectedRows.end && r2 >= settings.protectedRows.start) {
                                item.style.pointerEvents = "none";
                                item.style.opacity = 0.5;
                                item.title = "Protected rows";
                            }
                        }
                    });
                    menu.classList.add("custom-protection-applied");
                }
            });

            const target = document.querySelector(`#${getContainer()}-wa-container`);
            if (target) {
                observer.observe(target, { childList: true, subtree: true });
            }
        };

        const disableSheetRename = () => {
            const container = document.querySelector(`#${getContainer()}-sheets-item-container`);
            if (!container) return;

            container.addEventListener("dblclick", function (e) {
                if (!settings.allowRenameSheet &&
                    e.target.classList.contains("luckysheet-sheets-item-name")) {
                    e.stopImmediatePropagation();
                    e.preventDefault();
                }
            }, true);
        };

        const addCustomRightClickItems = () => {
            const observer = new MutationObserver(() => {
                const menu = document.querySelector("#luckysheet-cols-menu");
                if (menu && !menu.querySelector(".custom-items-added")) {
                    settings.customMenuItems.forEach((item) => {
                        const li = document.createElement("div");
                        li.className = "luckysheet-menus-item custom-menu-item";
                        li.innerHTML = `<span class="luckysheet-menus-item-content">${item.icon || ""} ${item.text}</span>`;
                        li.onclick = function () {
                            const cell = instance.getRange()[0];
                            item.action(cell.row[0], cell.column[0]);
                            menu.style.display = "none";
                        };
                        menu.appendChild(li);
                    });
                    menu.classList.add("custom-items-added");
                }
            });

            const target = document.querySelector(`#${getContainer()}-wa-container`);
            if (target) {
                observer.observe(target, { childList: true, subtree: true });
            }
        };

        // Apply behaviors
        blockDeleteProtectedRows();
        if (!settings.allowRenameSheet) disableSheetRename();
        if (settings.customMenuItems.length > 0) addCustomRightClickItems();
    };
})(jQuery);
