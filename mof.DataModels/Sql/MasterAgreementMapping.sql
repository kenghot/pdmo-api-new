select b.LOVCode as orgCode, b.LOVValue as orgName, a.LOVCode as projCode, a.LOVValue as projName
from (select * from tb.CE_LOV where LOVGroupCode = 'pjtype') a,
(select * from tb.CE_LOV where LOVGroupCode = 'orgtype') b
order by b.LOVCode,a.LOVCode
