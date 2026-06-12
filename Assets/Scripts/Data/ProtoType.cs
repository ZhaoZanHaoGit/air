public enum ProtoType : ushort
{
    Login = 456,
    Register = 457,
    SiginOut,
    Chat,
    AddUsers,
    GetUsers,
    DeleteUsers,
    UpdateUser,
    ForgotPassword,                    //忘记密码
    GetAllExamPapers,                  //获取所有试卷
    AddExamPaper,                      //添加试卷
    DeleteExamPaper,                   //删除试卷
    DeleteCurrentAllExamPapers,        //删除选择教师发布的所有试卷
    SubmitExamPaper,                   //考试试卷提交
    GetQuestion,                       //获取试题
    AddQuestions,                      //添加试题
    DeleteQuestions,                   //删除试题
    DeleteCurrentAllQuestions,         //删除选择教师发布的所有试题
    TeacherDeleteExamPaper,            //教师删除试卷
    UpdateExamPaper,                   //更新试卷
    GetCurrentAllExamResults,          //获取当前学生的所有考试结果
    GetAllExamResults,                 //获取所有学生的考试结果
    SubmitExamResult,                  //学生端提交考试成绩
    GetAllClassExamResults,            //获取教师管理班级学生成绩
    DeleteExamResults,                 //删除学生成绩
    AddCourseDurations,                //管理员端添加课表时间段
    GetAllCourseDurations,             //获取所有管理员发布的课表时间段
    DeleteCourseDurations,             //管理端删除所有课表时间段
    AddAndUpdateCourseDate,            //教师添加和更新课程表数据
    DeleteAllCourseDates,              //删除所有课程表
    GetAllCourseDatas,                 //获取所有老师添加的课程表数据
    GetAllPresetCourses,               //获取所有预设课程
    AddPresetCourseData,               //教师添加预设课程
    DeletePresetCourseData,            //教师删除预设课程
    DeletePresetCourseDatas,           //删除教师添加的所有预设课程
    GetAllCurrentUserPresetCourseData, //获取当前登录教师添加的所有预设课程 
    UpdatePresetCourseData,            //教师更新预设课程班级及人数及开启关闭
    AddTaskDBInfoData,                 //教师添加任务
    DeleteTaskDBInfoData,              //管理端删除任务
    GetAllCurrentUserTaskDBInfoDatas,  //获取当前登录老师添加的所有任务
    GetAllTaskDBInfoDatas,             //获取所有老师添加的任务
    UpdateTaskDBInfoData,              //更新任务
    SubmitTask,                        //学生提交任务
    AddTaskData,                       //学生上传任务作品数据
    UpdateTaskScore,                   //教师给学生作品评分
    DeleteTaskData,                    //删除学生作品
    GetAllCurrentUserTaskDatas,        //获取当前登录学生的所有作品信息
    GetAllTaskDatasByTeacher,          //获取当前教师发布所有任务学生提交的任务统计信息
    GetAllUserTaskDatas,               //获取所有学生的作品信息
    AddRescourceFolderData,            //教师端添加资源共享文件
    GetAllRescDatas,                   //获取当前学校所有老师添加的资源共享文件
    UpdateRescourceFolderData,         //跟新添加的资源共享文件数据
    DeleteRescourceFolderData,         //删除数据库资源共享数据
    AddAndUpdateSofControDate,         //管理员添加功能管理数据
    DeleteSofControDate,               //管理员删除功能管理数据
    GetSofControDate,                  //管理员删除功能管理数据
    AddCourseResourcesData,                       //教师端添加课程资源数据
    DeleteCourseResourcesData,                    //管理员删除教师添加课程资源数据
    DeleteCurrentAllCourseResourcesData,          //管理员删除多个教师添加课程资源数据
    TeacherDeleteCourseResourcesData,             //删除教师添加课程资源数据
    UpdateCourseResourcesData,                    //教师端更新课程资源数据
    GetAllCourseResourcesDatas,                   //获取所有课程资源数据
    AddSoftwareData,                              //添加软件
    DeleteSoftwareData,                           //删除软件
    UpdateSoftwareData,                           //更新软件数据
    GetAllSoftwareDatas,                          //获取所有软件数据
    GetAllStudentsLearnDatas,                     //获取学生学习数据
    GetAllLearnDatasByAccount,                    //学生获取自己的学习数据
    DeleteStudentsLearnDatas,                     //删除学生学习数据
    DeleteTaskDataByUsers,                        //删除学生任务
    AddSoftData = 526,                            //添加登录后更新软件登录状态
    QuitSoft,                                     //退出软件
    GetSoftStateOnline,                           //获取在线学生数量     
    AddSoftLearningData,                          //添加学生学习软件科目数据
    GetSoftLearnDatasByAccount,                   //获取学习数据根据自身账号
    GetSoftLearnDatasByClasses,
    GetUserByClasses,

    //数字人
    AddDigitalHumanData = 1000,                   //添加AI数字人数据
    UpdateDigitalHumanData,                       //更新数字人数据
    GetDigitalHumanData_Teacher,                  //教师获取数字人数据
    GetDigitalHumanData_Student,                  //学生获取数字人数据
    DeleteDigitalHumanData,                       //删除数字人数据
    GetDigitalHumanonStateData,                    //查询该班级是否开启了数字人服务

    //公共协议
    AddSoftResourcesDatas = 1200,                 //教师上传各个软件学习资源
    GetSoftResourcesDatas,
    DeleteResourcesDatas,
    AddSoftExamSettingData,                       //教师添加软件考核时长
    GetSoftExamSettingData,                        //获取软件考核时长
    AddTaskPostingData,                            //教师发布任务
    GetTeacherTasks,                               //获取教师发布的任务
    DeleteTaskPostingData,                         //删除教师发布的任务
    GetStudentTasks,                                //获取学生任务
    AddAgentData = 1209,                             //添加智能体
    GetAgentDatasByTeacher,                        //教师获取智能体
    GetAgentDatasByStudent,                        //学生获取智能体
    DeleteAgentDatas,
    UpdateAgentData
}
