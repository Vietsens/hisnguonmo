  --Danh sách bệnh nhân thực hiện giải phẫu bệnh lý theo phòng thực hiện
 SELECT 
VHR.DEPARTMENT_NAME REQUEST_DEPARTMENT,
VHR.ROOM_NAME REQUEST_ROOM,
DE.DEPARTMENT_NAME EXECUTE_DEPARTMENT_NAME,
EXRO.EXECUTE_ROOM_NAME,
SR.SERVICE_REQ_STT_ID,
SR.TDL_PATIENT_CODE,
SR.TDL_PATIENT_NAME,
T.TDL_PATIENT_DOB,
SS.TDL_SERVICE_CODE,
SS.TDL_SERVICE_NAME,
SR.BLOCK,
SSE.INSTRUCTION_NOTE,
SS.amount,
SR.REQUEST_USERNAME,
--SR.EXECUTE_USERNAME,
--SR.REQUEST_USERNAME,
SR.ICD_NAME,
--SR.FINISH_TIME,
SR.INTRUCTION_TIME
FROM HIS_SERE_SERV SS 
JOIN HIS_SERVICE_REQ SR ON SR.ID=SS.SERVICE_REQ_ID 
JOIN HIS_EXECUTE_ROOM EXRO ON EXRO.ROOM_ID=SR.EXECUTE_ROOM_ID 
JOIN HIS_DEPARTMENT DE ON DE.ID = SS.TDL_EXECUTE_DEPARTMENT_ID
JOIN V_HIS_ROOM VHR ON VHR.ID = SS.TDL_REQUEST_ROOM_ID
JOIN HIS_TREATMENT T ON T.ID=SS.TDL_TREATMENT_ID
LEFT JOIN HIS_SERE_SERV_EXT SSE ON SSE.TDL_SERVICE_REQ_ID = SR.ID
WHERE 
SS.IS_DELETE =0 
AND SS.IS_EXPEND IS NULL 
AND SS.IS_NO_EXECUTE IS NULL 
AND SR.IS_DELETE =0 
AND SR.IS_NO_EXECUTE IS NULL
AND sr.service_req_type_id = 13
AND SR.INTRUCTION_TIME BETWEEN :FINISH_TIME_FROM AND :FINISH_TIME_TO
AND (SR.EXECUTE_ROOM_ID IN (:EXECUTE_ROOM_IDs) OR '''' in (:EXECUTE_ROOM_IDs||''''))
;