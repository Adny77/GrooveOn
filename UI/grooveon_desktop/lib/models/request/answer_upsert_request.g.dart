// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'answer_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnswerUpsertRequest _$AnswerUpsertRequestFromJson(Map<String, dynamic> json) =>
    AnswerUpsertRequest(
      questionId: (json['questionId'] as num).toInt(),
      adminId: (json['adminId'] as num).toInt(),
      message: json['message'] as String,
    );

Map<String, dynamic> _$AnswerUpsertRequestToJson(
  AnswerUpsertRequest instance,
) => <String, dynamic>{
  'questionId': instance.questionId,
  'adminId': instance.adminId,
  'message': instance.message,
};
