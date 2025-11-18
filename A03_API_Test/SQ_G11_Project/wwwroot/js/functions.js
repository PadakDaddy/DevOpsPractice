/*
 * FILE          : functions.js
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 * DESCRIPTION   : Functions for validating variables
 */

const API_SERVER = "https://localhost:7191/api/tasks";

let currentMode = "add";

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function addTask() {
  const addForm = document.getElementById("addTaskForm");

  if (
    addForm.style.visibility === "hidden" ||
    addForm.style.visibility === ""
  ) {
    // Show form for new task entry
    currentMode = "add";
    clearFormInputs();
    addForm.style.visibility = "visible";

    disabledSubMenuForm();
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function updateTask() {
  disabledSubMenuForm();

  document.getElementById("taskId").disabled = true;
  document.getElementById("taskAssigneeId").disabled = true;
  document.getElementById("taskPriorityId").disabled = true;

  const checkedBoxes = document.querySelectorAll(".data-checkbox:checked");
  if (checkedBoxes.length !== 1) {
    alert("Please select exactly one task to update.");
    return;
  }

  // Show form and populate inputs with selected task data
  currentMode = "update";
  const selectedId = checkedBoxes[0].value;
  const dataArea = document.getElementById("dataAreaId");
  const rows = dataArea.getElementsByTagName("tr");

  let selectedRow;
  for (const row of rows) {
    const idCell = row.cells[1];
    if (idCell && idCell.textContent === selectedId) {
      selectedRow = row;
      break;
    }
  }

  if (!selectedRow) {
    alert("Selected task not found.");
    return;
  }

  document.getElementById("taskId").value = selectedId;
  document.getElementById("taskTitleId").value =
    selectedRow.cells[2].textContent;
  document.getElementById("taskAssigneeId").value =
    selectedRow.cells[3].textContent;
  document.getElementById("taskPriorityId").value =
    selectedRow.cells[4].textContent;

  // Show form for update
  const form = document.getElementById("addTaskForm");
  form.style.visibility = "visible";
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function updatePriority() {
  let newPriorityValue = document.getElementById("modTaskPriorityId").value;
  const errMsg = document.getElementById("priorityErrMsg");
  const selectedTask = document.querySelector(".data-checkbox:checked");
  document.getElementById("addTaskForm").style.visibility = "hidden";
  //Reset error message
  errMsg.textContent = "";

  //check if a task is selected
  if (!selectedTask) {
    errMsg.textContent = "Please select a task to update.";
    return;
  }

  const filterID = selectedTask.value;

  //validate input
  if (isNaN(newPriorityValue)) {
    errMsg.textContent = "Error: Priority must be a numeric value.";
    return;
  }
  const numericPriority = parseInt(newPriorityValue);
  if (numericPriority < 1 || numericPriority > 4) {
    errMsg.textContent = "Error: Priority must be between 1 and 4.";
    return;
  }
  if (numericPriority == selectedTask.id) {
    errMsg.textContent = "New priority matches existing priority.";
    return;
  }
  //use patch request to update priority
  try {
    const response = await fetch(`${API_SERVER}/${filterID}/priority`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify(numericPriority),
    });

    if (!response.ok) {
      console.error("API Error:", response.status, await response.text());
      alert("Failed to update task priority.");
      return;
    }
    //update list with new priority.
    await searItemByCondition();
    document.getElementById("modifyPriority").style.visibility = "hidden";
    alert("Update successful.");
  } catch (error) {
    console.error("Full error details:", error);
    errMsg.textContent = `Error: ${error.message}`;
  }
  newPriorityValue = 0;
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function saveTask() {
  const taskId = parseInt(document.getElementById("taskId").value);
  const priorityId = parseInt(document.getElementById("taskPriorityId").value);

  if (!isNumber(taskId) || taskId < 1) {
    alert("Task ID contains only number greater than 0");
    return;
  }

  if (!isNumber(priorityId)) {
    alert("Priority contains only number (0 ~ 4)");
    return;
  }

  if (currentMode === "add") {
    await addNewTask();
  } else if (currentMode === "update") {
    await updateExistingTask();
  }

  document.getElementById("taskId").disabled = false;
  document.getElementById("taskAssigneeId").disabled = false;
  document.getElementById("taskPriorityId").disabled = false;
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function addNewTask() {
  const id = parseInt(document.getElementById("taskId").value);
  const title = document.getElementById("taskTitleId").value.trim();
  const assignee = document.getElementById("taskAssigneeId").value.trim();
  const priority = parseInt(document.getElementById("taskPriorityId").value);

  // Basic validation
  if (!title || isNaN(id) || isNaN(priority)) {
    alert("Please enter valid ID, Title, and Priority.");
    return;
  }

  const taskData = { id, title, assignee, priority };

  try {
    const response = await fetch(API_SERVER, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(taskData),
    });

    if (!response.ok) {
      const errorMsg = await response.text();
      alert("Add failed: " + errorMsg);
      return;
    }

    alert("Task added successfully!");
    document.getElementById("addTaskForm").style.visibility = "hidden";
    clearFormInputs();
    await searItemByCondition(); // Refresh table
  } catch (error) {
    alert("Error adding task: " + error);
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function updateExistingTask() {
  const id = parseInt(document.getElementById("taskId").value);
  const title = document.getElementById("taskTitleId").value.trim();

  if (!title || isNaN(id)) {
    alert("Please enter valid ID and Title.");
    return;
  }

  const taskData = { id, title };

  try {
    const response = await fetch(`${API_SERVER}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(taskData),
    });

    if (!response.ok) {
      const errorMsg = await response.text();
      alert("Update failed: " + errorMsg);
      return;
    }

    alert("Task updated successfully!");
    document.getElementById("addTaskForm").style.visibility = "hidden";
    clearFormInputs();
    await searItemByCondition(); // Refresh data
  } catch (error) {
    alert("Error updating task: " + error);
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function cancelTask() {
  // Hide form and clear inputs
  document.getElementById("addTaskForm").style.visibility = "hidden";
  clearFormInputs();
  currentMode = "add";

  updateButtonStates();
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function removePriority() {
  const errMsg = document.getElementById("priorityErrMsg");
  const selectedTask = document.querySelector(".data-checkbox:checked");

  //Reset error message
  errMsg.textContent = "";

  //check if a task is selected
  if (!selectedTask) {
    errMsg.textContent = "Please select a task to update.";
    return;
  }
  const filterID = selectedTask.value;

  selectedTask.id = 0;
  //use patch request to update priority
  try {
    const response = await fetch(`${API_SERVER}/${filterID}/priority`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(selectedTask.id),
    });

    if (!response.ok) {
      console.error("API Error:", response.status, await response.text());
      alert("Failed to remove task priority.");
      return;
    }
    //clear input field and update list with new priority.
    await searItemByCondition();
    document.getElementById("modifyPriority").style.visibility = "hidden";
    alert("Successfully removed priority.");
  } catch (error) {
    console.error("Full error details:", error);
    errMsg.textContent = `Error: ${error.message}`;
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
async function deleteTask() {
  const checkedBoxes = document.querySelectorAll(".data-checkbox:checked");
  if (checkedBoxes.length === 0) {
    alert("Please select at least one task to delete.");
    return;
  }

  // Confirm before deleting
  if (!confirm("Are you sure you want to delete the selected task(s)?")) return;

  try {
    for (const checkbox of checkedBoxes) {
      const id = checkbox.value;
      const response = await fetch(`${API_SERVER}/${id}`, { method: "DELETE" });

      if (!response.ok) {
        const errorMsg = await response.text();
        alert(`Failed to delete task id ${id}: ${errorMsg}`);
        return;
      }
    }
    alert("Selected task(s) deleted successfully!");
    await searItemByCondition();
  } catch (error) {
    alert("Error deleting tasks: " + error);
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
// Helper function to clear form inputs
function clearFormInputs() {
  document.getElementById("taskId").value = "";
  document.getElementById("taskTitleId").value = "";
  document.getElementById("taskAssigneeId").value = "";
  document.getElementById("taskPriorityId").value = "";
}

//
// FUNCTION     : searItemByCondition
// DESCRIPTION  : Call the API indicating conditions
// PARAMETERS   : None
// RETURNS      : none
//
async function searItemByCondition() {
  let filterType = document.getElementById("filterTypeId").value;
  let filterData = document.getElementById("filterDataId").value;
  let filter = "";

  document.getElementById("addTaskForm").style.visibility = "hidden";
  document.getElementById("addTaskForm").style.visibility = "hidden";

  let filterDataNum = parseInt(filterData);
  if (filterType === "1" && !isNumber(filterDataNum)) {
    alert("The ID contains only number");
    return;
  }

  switch (filterType) {
    case "1":
      filter = "id";
      break;
    case "2":
      filter = "assignee";
      break;
    default:
      break;
  }

  if ((filterType == "1" || filterType == "2") && filterData.length == 0) {
    alert(filter + " must not be blank");
    return;
  }

  if (filterType != "0" && filterData.length != 0) {
    filter = "?" + filter + "=" + filterData;
  }

  const apiUrl = API_SERVER + filter;
  const response = await fetch(apiUrl);

  if (response.status != 200 && response.status != 204) {
    console.error("API Error:", response.status, await response.text());
    alert("Failed to load data");
    return;
  }

  if (response.status == 204) {
    await disPlayResult([]);
    return;
  }

  try {
    const resultArray = await response.json(); // .json() use
    await disPlayResult(resultArray);
  } catch (error) {
    console.error("JSON Parsing error: ", error);
  }
}

//
// FUNCTION     : disPlayResult
// DESCRIPTION  : Display task information
// PARAMETERS   : resultArray - An task list array from server
// RETURNS      : None
//
async function disPlayResult(resultArray) {
  const dataArea = document.getElementById("dataAreaId");

  dataArea.innerHTML = "";

  resultArray.forEach((rows) => {
    const row = document.createElement("tr");

    const checkBoxCell = document.createElement("td");
    const checkBox = document.createElement("input");
    checkBox.type = "radio";
    checkBox.className = "data-checkbox";
    checkBox.name = "selectedTask";
    checkBox.value = rows.id;
    checkBox.addEventListener("change", updateButtonStates);
    checkBoxCell.appendChild(checkBox);
    row.appendChild(checkBoxCell);

    const idCell = document.createElement("td");
    idCell.textContent = rows.id;
    row.appendChild(idCell);

    const titleCell = document.createElement("td");
    titleCell.textContent = rows.title;
    row.appendChild(titleCell);

    const assigneeCell = document.createElement("td");
    assigneeCell.textContent = rows.assignee ?? "";
    row.appendChild(assigneeCell);

    const priorityCell = document.createElement("td");
    priorityCell.textContent = rows.priority;
    row.appendChild(priorityCell);

    dataArea.appendChild(row);
  });
  updateButtonStates();
}

//
// FUNCTION     : updateButtonStates
// DESCRIPTION  : Change button status according to the page status. 
//                For example, menu button is clicked, radio button is selected.
// PARAMETERS   : None
// RETURNS      : None
//
function updateButtonStates() {
  const addTaskForm = document.getElementById("addTaskForm");
  const updateButton = document.getElementById("updateButtonId");
  const deleteButton = document.getElementById("deleteButtonId");
  const addAssigneeButton = document.getElementById("addAssigneeButtonId");
  const removeAssigneeButton = document.getElementById(
    "removeAssigneeButtonId"
  );
  const modifyPriority = document.getElementById("modifyPriority");
  const modTaskPriorityInput = document.getElementById("modTaskPriorityId");

  const checkedBoxes = document.querySelectorAll(".data-checkbox:checked");

  document.getElementById("taskId").disabled = false;
  document.getElementById("taskAssigneeId").disabled = false;
  document.getElementById("taskPriorityId").disabled = false;

  if (checkedBoxes.length === 1) {
    updateButton.disabled = false;
    deleteButton.disabled = false;
    modifyPriority.style.visibility = "visible";
    addTaskForm.style.visibility = "hidden";

    const selectedCheckbox = checkedBoxes[0];
    const selectedRow = selectedCheckbox.closest("tr");
    const assigneeCell = selectedRow.cells[3];
    const priorityCell = selectedRow.cells[4];

    if (priorityCell && priorityCell.textContent.trim() !== "") {
      modTaskPriorityInput.value = priorityCell.textContent.trim();
    } else {
      modTaskPriorityInput.value = "";
    }

    const assigneeVal = (assigneeCell.textContent || "").trim().toLowerCase();
    if (
      assigneeVal === "" ||
      assigneeVal === "null" ||
      assigneeVal === "undefined"
    ) {
      addAssigneeButton.style.display = "inline";
      removeAssigneeButton.style.display = "none";
    } else {
      addAssigneeButton.style.display = "none";
      removeAssigneeButton.style.display = "inline";
    }
  } else if (checkedBoxes.length > 1) {
    updateButton.disabled = true;
    deleteButton.disabled = false;
    modifyPriority.style.visibility = "hidden";
    modTaskPriorityInput.value = "";
    addAssigneeButton.style.display = "none";
    removeAssigneeButton.style.display = "none";
  } else {
    updateButton.disabled = true;
    deleteButton.disabled = true;
    modifyPriority.style.visibility = "hidden";
    modTaskPriorityInput.value = "";
    addAssigneeButton.style.display = "none";
    removeAssigneeButton.style.display = "none";
  }
}

//
// FUNCTION     :
// DESCRIPTION  :
// PARAMETERS   :
// RETURNS      :
//
document.addEventListener("DOMContentLoaded", () => {
  const dataArea = document.getElementById("dataAreaId");
  const selectBoxChanged = document.getElementById("filterTypeId");
  const filterDataBox = document.getElementById("filterDataId");

  dataArea.addEventListener("change", (event) => {
    if (event.target.classList.contains("data-checkbox")) {
      updateButtonStates();
    }
  });

  selectBoxChanged.addEventListener("change", (event) => {
    filterDataBox.value = "";
    switch (event.target.value) {
      case "0":
        filterDataBox.disabled = true;
        break;
      case "1":
      case "2":
        filterDataBox.disabled = false;
        break;
    }
  });
});

//----------------------------ADD ASSIGNEE FUNCTIONS------------------------
//
// FUNCTION     : getSelectedTaskId
// DESCRIPTION  : picks the selected task id
// PARAMETERS   : none
// RETURNS      : Id of the selected task, or null if no selection
//
function getSelectedTaskId() {
  const checked = document.querySelectorAll(".data-checkbox:checked");
  if (checked.length !== 1) return null;
  return checked[0].value;
}

//
// FUNCTION     : openAddAssigneeForSelected
// DESCRIPTION  : opens the small modal that asks for the assignee name
// PARAMETERS   : none
// RETURNS      : none
//
function openAddAssigneeForSelected() {
  const id = getSelectedTaskId();
  if (!id) {
    showAssigneeStatus("Please select exactly one task", "error");
    return;
  }
  document.getElementById("assigneeNameInput").value = "";
  const modal = document.getElementById("assigneeModal");
  modal.classList.remove("form-hidden");
  modal.setAttribute("aria-hidden", "false");
}

//
// FUNCTION     : submitAssignAssigneeForSelected
// DESCRIPTION  : Sends assignee name and task id to the backend to be saved
// PARAMETERS   : none
// RETURNS      : none
//
async function submitAssignAssigneeForSelected() {
  const taskId = getSelectedTaskId();
  const name = document.getElementById("assigneeNameInput").value.trim();

  if (!taskId) {
    showAssigneeStatus("Please select exactly one task", "error");
    return;
  }
  if (!name) {
    showAssigneeStatus("Assignee name is required", "error");
    return;
  }

  try {
    const url = `${API_SERVER}/${taskId}/assignee`;
    const resp = await fetch(url, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name }),
    });

    if (resp.ok) {
      closeAssigneeModal();
      await searItemByCondition(); // refresh table
      showAssigneeStatus(`Assigned "${name}" to task #${taskId}.`, "success");
    } else {
      const txt = await resp.text();
      showAssigneeStatus(txt || `Error: ${resp.status}`, "error");
    }
  } catch (e) {
    console.error(e);
    showAssigneeStatus("error while assigning.", "error");
  }
}

//
// FUNCTION     : closeAssigneeModal
// DESCRIPTION  : hides popup window after adding or cancelling assignee
// PARAMETERS   : none
// RETURNS      : none
//
function closeAssigneeModal() {
  const modal = document.getElementById("assigneeModal");
  modal.classList.add("form-hidden");
  modal.setAttribute("aria-hidden", "true");
}

//
// FUNCTION     : showAssigneeStatus
// DESCRIPTION  : tells teh user that has been successful with assigning or an error
// PARAMETERS   : msg, type
// RETURNS      : none
//
function showAssigneeStatus(msg, type = "success") {
  const box = document.getElementById("assigneeStatus");
  if (!box) return;
  //shows message when assignee is adde d to task
  box.textContent = msg;
  box.style.display = "block";
  box.style.color = type === "error" ? "red" : "green";
  //hides message after 3 seconds
  setTimeout(() => {
    box.style.display = "none";
  }, 3000);
}

//---------------------------REMOVEW ASSIGNEE FUNCTIONS--------------------

let __removeTargetTaskId = null; // saves the task id when user clicks remove assignee
// so we know which one to delete when they press yes

//
// FUNCTION     : openRemoveAssigneeConfirm
// DESCRIPTION  : opens confirm modal
// PARAMETERS   : taskId
// RETURNS      : none
//
function openRemoveAssigneeConfirm(taskId) {
  __removeTargetTaskId = taskId;
  const modal = document.getElementById("assigneeRemoveModal");
  modal.classList.remove("form-hidden");
  modal.setAttribute("aria-hidden", "false");
}

//
// FUNCTION     : closeRemoveAssigneeModal
// DESCRIPTION  : close confirm modal
// PARAMETERS   : none
// RETURNS      : none
//
function closeRemoveAssigneeModal() {
  const modal = document.getElementById("assigneeRemoveModal");
  modal.classList.add("form-hidden");
  modal.setAttribute("aria-hidden", "true");
  __removeTargetTaskId = null;
}

//
// FUNCTION     : submitRemoveAssigneeConfirmed
// DESCRIPTION  : user clicked "Yes" in the confirm modal
// PARAMETERS   : none
// RETURNS      : none
//
async function submitRemoveAssigneeConfirmed() {
  const taskId = __removeTargetTaskId;
  if (!taskId) {
    closeRemoveAssigneeModal();
    return;
  }

  try {
    const url = `${API_SERVER}/${taskId}/assignee`;
    const resp = await fetch(url, { method: "DELETE" });

    if (resp.status === 204) {
      closeRemoveAssigneeModal();
      await searItemByCondition(); //refresh table
      showAssigneeStatus(`Assignee removed from task #${taskId}.`, "success");
    } else {
      const txt = await resp.text();
      showAssigneeStatus(txt || `Error: ${resp.status}`, "error");
    }
  } catch (e) {
    console.error(e);
    showAssigneeStatus("error while removing assignee.", "error");
  }
}

//
// FUNCTION     : removeAssigneeForSelected
// DESCRIPTION  : user clicked the Remove button under the main actions
// PARAMETERS   : none
// RETURNS      : none
//
async function removeAssigneeForSelected() {
  const taskId = getSelectedTaskId();
  if (!taskId) {
    showAssigneeStatus("Please select exactly one task.", "error");
    return;
  }
  openRemoveAssigneeConfirm(taskId);
}

//
// FUNCTION     : disabledSubMenuForm
// DESCRIPTION  : Disable the add form when the user aborts editing or selects another menu.
// PARAMETERS   : none
// RETURNS      : none
//
function disabledSubMenuForm() {
  document.getElementById("removeAssigneeButtonId").style.display = "none";
  document.getElementById("modifyPriority").style.visibility = "hidden";
  document.getElementById("addAssigneeButtonId").style.display = "none";
}
