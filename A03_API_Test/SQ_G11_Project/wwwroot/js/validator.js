/*
 * FILE          : validator.js
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 * DESCRIPTION   : Functions for validating variables
 */

function isNumber(value) {
  return !isNaN(value) && value !== "";
}
