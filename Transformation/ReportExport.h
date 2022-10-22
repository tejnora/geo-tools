struct Report
{
	Report()
		:mNullity(false),mSumOfNormalEquationsAbsoluteResiduals(-1),mConditionNumber(-1),mNumberOfIdenticalPoints(-1), mStandardDeviation(-1)
		,mStandardDeviationInCoordinates(-1),mNumberOfIterations(-1),mSuccess(false){}
	struct Correlation
	{//"Maximal correlation is: "<<max_cor.cor<<" between parameters: "<<max_cor.i<<' '<<max_cor.j;
		Correlation()
			:mMaximal(-1),mStart(-1),mEnd(-1){}
		double mMaximal;//Maximal correlation is
		int mStart;
		int mEnd;
		GNU_gama::Mat<> mMatrix;
	};

	enum UsedSolutionAlgorithms
	{
		usaGaussJordanElimination=0,
		usaSvd=1
	};


	double mKey[8];
	int mNumberOfIdenticalPoints;
	double mStandardDeviation;
	double mStandardDeviationInCoordinates;
	int mNumberOfIterations;
	bool mSuccess;
	UsedSolutionAlgorithms mUsedSolutionAlgorithm;
	bool mNullity;//"Coefficient matrix of project equations is ill-conditioned or singular, the number of matrix defect is:nullity "
	double mSumOfNormalEquationsAbsoluteResiduals;//"The sum of normal equations absolute residuals: "
	int mConditionNumber;
	Correlation mCorrelation;
	GNU_gama::Mat<> mResiduals;
};

extern "C" _declspec(dllexport) int* _stdcall Report_Keys(Report* THIS)
{
	return (int*)(THIS->mKey);
}

extern "C" _declspec(dllexport) int _stdcall Report_NumberOfIdenticalPoints(Report* THIS)
{
	return THIS->mNumberOfIdenticalPoints;
}

extern "C" _declspec(dllexport) double _stdcall Report_StandardDeviation(Report* THIS)
{
	return THIS->mStandardDeviation;
}

extern "C" _declspec(dllexport) double _stdcall Report_StandardDeviationInCoordinates(Report* THIS)
{
	return THIS->mStandardDeviationInCoordinates;
}

extern "C" _declspec(dllexport) int _stdcall Report_NumberOfIterations(Report* THIS)
{
	return THIS->mNumberOfIterations;
}


extern "C" _declspec(dllexport) bool _stdcall Report_Success(Report* THIS)
{
	return THIS->mSuccess;
}

extern "C" _declspec(dllexport) int _stdcall Report_UsedSolutionAlgorithm(Report* THIS)
{
	return THIS->mUsedSolutionAlgorithm;
}


extern "C" _declspec(dllexport) bool _stdcall Report_Nullity(Report* THIS)
{
	return THIS->mNullity;
}

extern "C" _declspec(dllexport) double _stdcall Report_SumOfNormalEquationsAbsoluteResiduals(Report* THIS)
{
	return THIS->mSumOfNormalEquationsAbsoluteResiduals;
}

extern "C" _declspec(dllexport) int _stdcall Report_ConditionNumber(Report* THIS)
{
	return THIS->mConditionNumber;
}

extern "C" _declspec(dllexport) int _stdcall Report_Residuals_Size(Report* THIS)
{
	return THIS->mResiduals.end()-THIS->mResiduals.begin();
}

extern "C" _declspec(dllexport) int* _stdcall Report_Residuals_Data(Report* THIS)
{
	return (int*)THIS->mResiduals.begin();
}

extern "C" _declspec(dllexport) void _stdcall Report_Release(Report* THIS)
{
	delete THIS;
}
