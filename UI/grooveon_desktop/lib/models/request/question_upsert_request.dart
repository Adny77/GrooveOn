import 'package:json_annotation/json_annotation.dart';

part 'question_upsert_request.g.dart';

@JsonSerializable()
class QuestionUpsertRequest {
  final int userId;
  final String title;
  final String content;
  final String status;
  final String? answer;

  QuestionUpsertRequest({
    required this.userId,
    required this.title,
    required this.content,
    this.status = "Pending",
    this.answer,
  });

  factory QuestionUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$QuestionUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$QuestionUpsertRequestToJson(this);
}