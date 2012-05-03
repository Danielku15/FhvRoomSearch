
	
SELECT a.RoomId, COUNT(c.Id) AS ConflictingCourses, d.Id AS CurrentCourseId
FROM RoomSet AS a 
	LEFT OUTER JOIN RoomCourse AS b ON b.Rooms_Id = a.Id
	-- Conflicting Courses
	LEFT OUTER JOIN CourseSet AS c ON c.Id = b.Course_Id AND '2011-11-15 12:00:00' <= c.EndTime AND '2011-11-15 15:00:00' >= c.StartTime
	-- Current Course
	LEFT OUTER JOIN CourseSet AS d ON d.Id = b.Course_Id AND '2011-11-15 11:40:00' <= d.EndTime AND '2011-11-15 11:40:00' >= d.StartTime 
	-- Min Time
	LEFT OUTER JOIN 
		(SELECT MIN(g.StartTime) as MinTime FROM RoomCourse f 
		 INNER JOIN CourseSet g ON g.Id = f.Course_Id AND '2011-11-15 11:40:00' >= e.StartTime
	     WHERE f.Rooms_Id = a.RoomId) AS h
	-- Next Course
	LEFT OUTER JOIN CourseSet AS e ON e.Id = b.Course_Id AND e.StartTime = h.MinTime
WHERE (a.Id IN (29,30,31,32,33,34,35,36,37,38))
GROUP BY a.RoomId, d.Id	

SELECT a.RoomId, COUNT(c.Id) AS ConflictingCourses, d.Id AS CurrentCourseId
FROM RoomSet AS a 
	LEFT OUTER JOIN RoomCourse AS b ON b.Rooms_Id = a.Id
	-- Conflicting Courses
	LEFT OUTER JOIN CourseSet AS c ON c.Id = b.Course_Id AND '2011-11-15 12:00:00' <= c.EndTime AND '2011-11-15 15:00:00' >= c.StartTime
	-- Current Course
	LEFT OUTER JOIN CourseSet AS d ON d.Id = b.Course_Id AND '2011-11-15 11:40:00' <= d.EndTime AND '2011-11-15 11:40:00' >= d.StartTime 
WHERE (a.Id IN (29,30,31,32,33,34,35,36,37,38))
GROUP BY a.RoomId, d.Id


	
SELECT a.RoomId, COUNT(c.Id) AS ConflictingCourses, d.Id AS CurrentCourseId
FROM RoomSet AS a 
	LEFT OUTER JOIN RoomCourse AS b ON b.Rooms_Id = a.Id
	-- Conflicting Courses
	LEFT OUTER JOIN CourseSet AS c ON c.Id = b.Course_Id AND @startTime <= c.EndTime AND @endTime >= c.StartTime
	-- Current Course 
	LEFT OUTER JOIN CourseSet AS d ON d.Id = b.Course_Id AND @now <= d.EndTime AND @now >= d.StartTime 
    -- All Upcoming Courses
	LEFT OUTER JOIN CourseSet AS e ON e.Id = b.Course_Id AND e.StartTime >= @now
WHERE (a.Id IN (29,30,31,32,33,34,35,36,37,38))
GROUP BY a.RoomId, d.Id

SELECT a.Id, COUNT(c.Id) AS ConflictingCourses, d.Id AS CurrentCourseId, MIN(e.startTime) NextCourseStart
FROM Rooms AS a 
    LEFT OUTER JOIN RoomCourses AS b ON b.RoomId = a.Id
    -- Conflicting Courses
    LEFT OUTER JOIN Courses AS c ON c.Id = b.CourseId AND @startTime <= c.EndTime AND @endTime >= c.StartTime
    -- Current Course  
    LEFT OUTER JOIN Courses AS d ON d.Id = b.CourseId AND @now <= d.EndTime AND @now >= d.StartTime 
    -- All Upcoming Courses
    LEFT OUTER JOIN CourseSet AS e ON e.Id = b.CourseId AND e.StartTime >= @now
WHERE (a.Id IN @rooms)
GROUP BY a.RoomId, d.Id