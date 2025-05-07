-- How can you retrieve all the information from the cd.facilities table?
SELECT * from cd.facilities

-- You want to print out a list of all of the facilities and their cost to members. How would you retrieve a list of only facility names and costs?
SELECT name, membercost FROM cd.facilities

-- How can you produce a list of facilities that charge a fee to members?
SELECT * FROM cd.facilities WHERE membercost >0

-- How can you produce a list of facilities that charge a fee to members, and that fee is less than 1/50th of the monthly maintenance cost? Return the facid, facility name, member cost, and monthly maintenance of the facilities in question.
select facid,name,membercost,monthlymaintenance from cd.facilities where membercost<(monthlymaintenance/50.0) and membercost>0

-- How can you produce a list of all facilities with the word 'Tennis' in their name?
select * from cd.facilities where name like '%Tennis%'

-- How can you retrieve the details of facilities with ID 1 and 5? Try to do it without using the OR operator.
select * from cd.facilities where facid in (1,5)

-- How can you produce a list of facilities, with each labelled as 'cheap' or 'expensive' depending on if their monthly maintenance cost is more than $100? Return the name and monthly maintenance of the facilities in question.
select name,
case when (monthlymaintenance>100) then
'expensive'
else
'cheap'
end as cost
from cd.facilities

-- How can you produce a list of members who joined after the start of September 2012? Return the memid, surname, firstname, and joindate of the members in question.
select memid, surname, firstname, joindate from cd.members where joindate >='2012-09-01 00:01:00'

-- How can you produce an ordered list of the first 10 surnames in the members table? The list must not contain duplicates.
select distinct surname from cd.members order by surname asc limit 10

-- You, for some reason, want a combined list of all surnames and all facility names. Yes, this is a contrived example :-). Produce that list!
select name from cd.facilities
union
select surname from cd.members

-- You'd like to get the signup date of your last member. How can you retrieve this information?
select max(joindate) as joindate from cd.members

-- You'd like to get the first and last name of the last member(s) who signed up - not just the date. How can you do that?
select firstname,surname,joindate from cd.members where joindate in (select max(joindate) from cd.members)

-- How can you produce a list of the start times for bookings by members named 'David Farrell'?
select starttime from cd.bookings join cd.members on cd.members.memid=cd.bookings.memid
where firstname ='David' and surname ='Farrell'

-- How can you produce a list of the start times for bookings for tennis courts, for the date '2012-09-21'? Return a list of start time and facility name pairings, ordered by the time.
select starttime as start,name from cd.bookings inner join cd.facilities on cd.facilities.facid=cd.bookings.facid
where cd.facilities.name in ('Tennis Court 1','Tennis Court 2') and starttime>= '2012-09-21' and starttime < '2012-09-22' 
order by starttime

-- How can you output a list of all members who have recommended another member? Ensure that there are no duplicates in the list, and that results are ordered by (surname, firstname).
select distinct recs.firstname as firstname, recs.surname as surname
	from 
		cd.members mems
		inner join cd.members recs
			on recs.memid = mems.recommendedby
order by surname, firstname;       

-- How can you output a list of all members, including the individual who recommended them (if any)? Ensure that results are ordered by (surname, firstname).
select mems.firstname as memfname, mems.surname as memsname, recs.firstname as recfname,recs.surname as recsname
	from 
		cd.members mems
		left outer join cd.members recs
			on recs.memid = mems.recommendedby
order by memsname, memfname;       

-- How can you produce a list of all members who have used a tennis court? Include in your output the name of the court, and the name of the member formatted as a single column. Ensure no duplicate data, and order by the member name followed by the facility name.
select distinct CONCAT(mem.firstname,' ',mem.surname) as member, fcs.name as facility from cd.members mem 
inner join cd.bookings bks on bks.memid = mem.memid
inner join cd.facilities fcs on fcs.facid=bks.facid where fcs.name like 'Tennis%' order by member, fcs.name

-- How can you produce a list of bookings on the day of 2012-09-14 which will cost the member (or guest) more than $30? Remember that guests have different costs to members (the listed costs are per half-hour 'slot'), and the guest user is always ID 0. Include in your output the name of the facility, the name of the member formatted as a single column, and the cost. Order by descending cost, and do not use any subqueries.
select mems.firstname || ' ' || mems.surname as member, 
	facs.name as facility, 
	case 
		when mems.memid = 0 then
			bks.slots*facs.guestcost
		else
			bks.slots*facs.membercost
	end as cost
        from
                cd.members mems                
                inner join cd.bookings bks
                        on mems.memid = bks.memid
                inner join cd.facilities facs
                        on bks.facid = facs.facid
        where
		bks.starttime >= '2012-09-14' and 
		bks.starttime < '2012-09-15' and (
			(mems.memid = 0 and bks.slots*facs.guestcost > 30) or
			(mems.memid != 0 and bks.slots*facs.membercost > 30)
		)
order by cost desc;          

-- How can you output a list of all members, including the individual who recommended them (if any), without using any joins? Ensure that there are no duplicates in the list, and that each firstname + surname pairing is formatted as a column and ordered.
SELECT DISTINCT 
    mem.firstname || ' ' || mem.surname AS member,
    rec.firstname || ' ' || rec.surname AS recommender
FROM cd.members mem
LEFT JOIN cd.members rec ON mem.recommendedby = rec.memid
ORDER BY member;

