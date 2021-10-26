using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupChooser
{
  public class Program
  {
    static void Main(string[] args)
    {
      var random = new Random();

      var students = new List<Student> {
        new Student(0),
        new Student(1),
        new Student(2),
        new Student(3),
        new Student(4),
        new Student(5),
        new Student(6),
        new Student(7),
        new Student(8),
        new Student(9),
        new Student(10),
        new Student(11),
        new Student(12),
        new Student(13),
        new Student(14),
        new Student(15),
        new Student(16),
        new Student(17),
        new Student(18),
        new Student(19),
        new Student(20),
        new Student(21),
        new Student(22),
        new Student(23),
        new Student(24),
        new Student(25),
        new Student(26),
        new Student(27),
        new Student(28),
        new Student(29)//,
        //new Student(30),
        //new Student(31)
      };

      var groupSize = 4;
      var iterationsToRun = 3;
      var results = new List<Iteration>();

      var currentIteration = 0;
      while (currentIteration < iterationsToRun)
      {
        // Get the list of students to work with. If we've grouped before, then we need to use those same students again, since they remember previous groups.
        List<Student> ungroupedStudents = results.Any() ? results.Last().Groups.SelectMany(x => x.Students).ToList() : students; // TODO: ensure that this is a shallow copy.

        var groups = new List<Group>();
        // Group Students
        while (ungroupedStudents.Any())
        {
          var nextBatchOfUngroupedStudents = new List<Student>();

          // HACK - since I know we want groups of 4, I can optimize a bit to fix situations where the # of students isn't divisible by 4:
          var desiredGroupSize = ungroupedStudents.Count < 10 && ungroupedStudents.Count % 3 == 0 ? 3 : groupSize;
          
          while (nextBatchOfUngroupedStudents.Count < desiredGroupSize && ungroupedStudents.Any())
          {
            // pick a student from ungrouped that doesn't collide with any already in the group
            var student = ungroupedStudents.FirstOrDefault(x => !nextBatchOfUngroupedStudents.Any(y => y.PreviouslyGroupedWithStudentIds.Contains(x.Id)));
            if (student == null)
            {
              // Naive solution: Pick an existing group at random to break apart and add back to the ungrouped pool, then try again?
              var randomIndex = random.Next(0, groups.Count);

              var groupToBreakUp = groups.ElementAt(randomIndex);
              var studentsToAddBackToUngroupedPool = groupToBreakUp.Students;
              foreach (var studentToPutBackInUngroupedPool in studentsToAddBackToUngroupedPool)
              {
                nextBatchOfUngroupedStudents.Remove(studentToPutBackInUngroupedPool);
                ungroupedStudents.Add(studentToPutBackInUngroupedPool);
              }
              groups.Remove(groupToBreakUp);
            }
            else
            {
              nextBatchOfUngroupedStudents.Add(student);
              ungroupedStudents.Remove(student);
            }
          }

          groups.Add(new Group(currentIteration, nextBatchOfUngroupedStudents));
        }

        results.Add(new Iteration(currentIteration, groups));

        // Update each Student so they rememeber which other Students they have been grouped with in the past.
        // Note: this logic isn't necessary if we're on the last iteration :shrug:
        foreach (var iteration in results)
        {
          foreach (var group in iteration.Groups)
          {
            foreach (var student in group.Students)
            {
              student.PreviouslyGroupedWithStudentIds.AddRange(group.Students.Where(x => x.Id != student.Id).Select(x => x.Id));
            }
          }
        }

        Console.WriteLine($"----------Iteration {currentIteration} Complete------------");
        foreach (var group in groups)
        {
          Console.WriteLine("Students: " + string.Join(",", group.Students.Select(x => x.Id).ToList()));
        }
        Console.WriteLine($"----------- END OF ITERATION {currentIteration} ------------");


        currentIteration++;
      }
    }
  }

  public class Iteration
  {
    public int Id { get; set; }
    public List<Group> Groups { get; set; }

    public Iteration(int id, List<Group> groups)
    {
      Id = id;
      Groups = groups;
    }
  }

  public class Group
  {
    public int Key { get; set; }
    public List<Student> Students { get; set; }

    public Group(int key, List<Student> students)
    {
      Key = key;
      Students = students;
    }
  }

  public class Student
  {
    public int Id { get; set; }
    public List<int> PreviouslyGroupedWithStudentIds { get; set; }

    public Student(int id)
    {
      Id = id;
      PreviouslyGroupedWithStudentIds = new List<int>();
    }

    public Student(int id, List<int> previouslyGroupedWithStudentIds)
    {
      Id = id;
      PreviouslyGroupedWithStudentIds = previouslyGroupedWithStudentIds;
    }
  }
}
