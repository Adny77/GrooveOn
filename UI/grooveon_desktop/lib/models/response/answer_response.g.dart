// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'answer_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AnswerResponse _$AnswerResponseFromJson(Map<String, dynamic> json) =>
    AnswerResponse(
      id: (json['id'] as num).toInt(),
      questionId: (json['questionId'] as num).toInt(),
      questionTitle: json['questionTitle'] as String?,
      adminId: (json['adminId'] as num).toInt(),
      adminUserName: json['adminUserName'] as String?,
      message: json['message'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$AnswerResponseToJson(AnswerResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'questionId': instance.questionId,
      'questionTitle': instance.questionTitle,
      'adminId': instance.adminId,
      'adminUserName': instance.adminUserName,
      'message': instance.message,
      'createdAt': instance.createdAt.toIso8601String(),
    };
