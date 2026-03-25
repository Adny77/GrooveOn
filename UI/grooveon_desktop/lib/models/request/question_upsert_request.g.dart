// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'question_upsert_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

QuestionUpsertRequest _$QuestionUpsertRequestFromJson(
  Map<String, dynamic> json,
) => QuestionUpsertRequest(
  userId: (json['userId'] as num).toInt(),
  title: json['title'] as String,
  content: json['content'] as String,
  status: json['status'] as String? ?? "Pending",
  answer: json['answer'] as String?,
);

Map<String, dynamic> _$QuestionUpsertRequestToJson(
  QuestionUpsertRequest instance,
) => <String, dynamic>{
  'userId': instance.userId,
  'title': instance.title,
  'content': instance.content,
  'status': instance.status,
  'answer': instance.answer,
};
