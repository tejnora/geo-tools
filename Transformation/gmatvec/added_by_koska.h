#ifndef GNU_gama_gMatVec_Added_by_koska__h_
#define GNU_gama_gMatVec_Added_by_koska__h_

#include "bandmat.h"

namespace GNU_gama {

//addition to matvec/bandmat.h
template <typename Float, typename Exc>
BandMat<Float, Exc>&
operator*(BandMat<Float, Exc> &A, const Float n)
{
	Mat<Float, Exc>::iterator a = A.begin();
	Index all=A.dim()*(A.bandWidth()+1);
	for (Index i=1; i<=all; i++)
		*a++ = (*a)*n;
	return A;
}

template <typename Float, typename Exc>
Mat<Float, Exc>
operator*(const BandMat<Float, Exc> &A, const Mat<Float, Exc> &B)
{
	if (A.dim() != B.rows())
		throw Exc(Exception::BadRank, "Mat operator*(const BandMat&, const Mat&)");

	int band=A.bandWidth();
	Mat<Float, Exc> C(A.dim(), B.cols());
	C.set_zero();
	Mat<Float, Exc>::iterator c = C.begin();
	Float s;

	for (long i=1; i<=C.rows(); i++)
		for (long j=1; j<=C.cols(); j++)
		{
			s = 0;
			for (long k=i-band; k<=i+band; k++)
			{				
				if(k<=0||k>A.dim()) continue;
				else s += A(i,k) * B(k,j);
			}
			*c++ = s;
		}		
	return C;
} 

/*
template <typename Float, typename Exc>
void extended_assignment_function(BandMat<Float, Exc>& A, const BandMat<Float, Exc>& B) 
{
   if ( B.bandWidth() == A.bandWidth() && A.dim() > B.dim() )
   {
		Mat<Float, Exc>::iterator a;
		Index band = A.bandWidth();			
		Mat<Float, Exc>::const_iterator b = B.begin();
		A.set_zero();       
				      
		for (Index i=1; i<=B.dim(); i++)
		{
			a = (A.begin()+(i-1)*(band+1));
			for (Index j=1; j<=(band+1); j++)
					if ( i+j-1 <= B.dim() )	*a++ = *b++;  
					else	{a++; b++;}
		}		
   }
   else A = B;     
}  
*/
// addition to matvec/vec.h
template <typename Float, typename Exc>
Float 
operator*(const Vec<Float, Exc> &A, const Vec<Float, Exc> &B)
{
    if (A.dim() != B.dim())
      throw Exc(Exception::BadRank, "Float operator*(const Vec&, const Vec&)");    
    Float s = 0;	
	typename Vec<Float, Exc>::const_iterator a = A.begin();
    typename Vec<Float, Exc>::const_iterator b = B.begin();    
    typename Vec<Float, Exc>::const_iterator e = A.end();
    while (a != e)
		s += (*a++) * (*b++); 
	return s;
}   

}   // namespace GNU_gama

#endif
