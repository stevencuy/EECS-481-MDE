// 
// Sample face detection code for PCSDK 
//


#include <iostream>
#include <Windows.h>
#include "util_pipeline.h"


using namespace std;


int main(int argc, wchar_t* argv[]) {

	map<PXCFaceAnalysis::Landmark::Label, string> faceLandmarkNames;
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_LEFT_EYE_INNER_CORNER] = "left eye inner";
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_LEFT_EYE_OUTER_CORNER] = "left eye outer";
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_RIGHT_EYE_INNER_CORNER] = "right eye inner";
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_RIGHT_EYE_OUTER_CORNER] = "right eye outer";
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_MOUTH_LEFT_CORNER] = "mouth left";
	faceLandmarkNames[PXCFaceAnalysis::Landmark::LABEL_MOUTH_RIGHT_CORNER] = "mouth right";


	UtilPipeline pipeline;
	PXCFaceAnalysis *faceAnalysis;

	pipeline.EnableFaceLocation();
	pipeline.EnableFaceLandmark();
	pipeline.Init();


	while(1)
	{
		if(pipeline.AcquireFrame(false))
		{
			faceAnalysis = pipeline.QueryFace();
			// get the face id for the first face
			pxcUID faceId;
			if(faceAnalysis->QueryFace(0, &faceId) != PXC_STATUS_ITEM_UNAVAILABLE)
			{
				// get the face location
				PXCFaceAnalysis::Detection *detector = faceAnalysis->DynamicCast<PXCFaceAnalysis::Detection>();
				PXCFaceAnalysis::Detection::Data faceData;
				if(detector->QueryData(faceId, &faceData) != PXC_STATUS_ITEM_UNAVAILABLE)
					cout << "\nFace " << faceId << " at location " << faceData.rectangle.x << " " << faceData.rectangle.y << endl;

				PXCFaceAnalysis::Landmark *landmarkDetector = faceAnalysis->DynamicCast<PXCFaceAnalysis::Landmark>();
				PXCFaceAnalysis::Landmark::LandmarkData landmarkData;
				PXCFaceAnalysis::Landmark::PoseData poseData;
				//Set Profile
				PXCFaceAnalysis::Landmark::ProfileInfo pinfo;
				pxcStatus sts = landmarkDetector->QueryProfile(0, &pinfo);
				pinfo.labels = PXCFaceAnalysis::Landmark::LABEL_7POINTS;
				sts = landmarkDetector->SetProfile(&pinfo);
				int i=0;
				cout << "faceId = " << faceId << endl;
				/*while(landmarkDetector->QueryPoseData(faceId, &poseData) != PXC_STATUS_ITEM_UNAVAILABLE)
				{
					Sleep(2000);
					cout << "fid = " << poseData.fid << " yaw = " << poseData.yaw << 
							" pitch =  " << poseData.pitch << " roll =  " << poseData.roll << endl;
				} */
				while(landmarkDetector->QueryLandmarkData(faceId, pinfo.labels, i++, &landmarkData) != PXC_STATUS_ITEM_UNAVAILABLE)
				{
					cout << "\nFace landmark " << faceLandmarkNames[landmarkData.label] << 
							" at x = " << landmarkData.position.x << 
							" y = " << landmarkData.position.y;
					if(landmarkDetector->QueryPoseData(faceId, &poseData) != PXC_STATUS_ITEM_UNAVAILABLE)
					{
					//Roll yaw pitch
						cout << " fid = " << poseData.fid << " yaw = " << poseData.yaw << 
						" pitch =  " << poseData.pitch << " roll =  " << poseData.roll << endl;
					}
					Sleep(1000);
				}
				/*void FaceRender::PrintLandmarkData7(PXCFaceAnalysis::Landmark *landmark, pxcU32 fid) {

    FaceRender::FaceData& itr=Insert(fid);
    wprintf_s(L"Landmark data fid=%d:\n", fid);
    for (int i=0;i<sizeof(itr.landmark)/sizeof(itr.landmark[0]);i++) {
        PXCFaceAnalysis::Landmark::LandmarkData data;
        pxcStatus sts=landmark->QueryLandmarkData(fid,labels[i],0,&data);
        if (sts<PXC_STATUS_NO_ERROR) continue;
        wprintf_s(L"%S : x=%4.1f, y=%4.1f z=%4.1f\n", landmarkLabels[i], data.position.x, data.position.y, data.position.z);

        PXCFaceAnalysis::Landmark::PoseData poseData;
        pxcStatus posests=landmark->QueryPoseData(fid, &poseData);
        // if (posests<PXC_STATUS_NO_ERROR) continue;
        wprintf_s(L"%S : yaw=%4.1f, roll=%4.1f pitch=%4.1f\n", "PoseData", poseData.yaw, poseData.roll, poseData.pitch);

    }
    wprintf_s(L"\n");
}*/

			}

			
			pipeline.ReleaseFrame();
		}
	}
}
